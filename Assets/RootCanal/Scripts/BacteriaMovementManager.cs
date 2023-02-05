#nullable enable

using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace RootCanal
{
    public class BacteriaMovementManager : MonoBehaviour
    {
        private struct Goal
        {
            public Vector3Int Cell;
            public Vector3 WorldPosition;

            public Goal(Vector3Int cell, Vector3 worldPosition)
            {
                Cell = cell;
                WorldPosition = worldPosition;
            }
        }

        private Bacterium? _selectedBacterium;
        private readonly Dictionary<Bacterium, Goal> _bacteriaGoals = new();
        private readonly HashSet<Bacterium> _bacteriaStopped = new();

        [Required] public Tilemap? Tilemap;
        [Required] public BacteriaManager? BacteriaManager;
        [Required] public Camera? Camera;
        public string SelectButton = "Fire1";
        public string SetGoalButton = "Fire2";
        public LayerMask SelectLayerMask;
        [Min(0f)] public float Speed = 0.1f;   //TODO: give player the ability to upgrade this
        [Min(0.001f)] public float MinOffsetFromGoal = 0.05f;
        public Vector3Int GoalTilePos;
        public bool Logging = false;
        public UnityEvent<(Bacterium, Vector3Int)> DestinationReached = new();

        private void Awake()
        {
            BacteriaManager!.BacteriumAdded.AddListener(bacterium => { });
        }

        private void setSelection(Bacterium? newSelection)
        {
            if (newSelection == _selectedBacterium)
                return;

            if (_selectedBacterium != null) {
                Debug.Log($"Deselecting bacterium {_selectedBacterium.name}");
                _selectedBacterium.Deselected.Invoke();
            }

            _selectedBacterium = newSelection;
            if (_selectedBacterium != null) {
                Debug.Log($"Selecting bacterium {_selectedBacterium.name}");
                _selectedBacterium.Selected.Invoke();
            }
        }

        private bool isAtLeastOneAdjacentTileMineable()
        {
            //get all the adjacent cells
            //check each cell to see if it has a "tile" in it (tile = minable piece of tooth)
            Vector3Int pos = Tilemap!.WorldToCell(transform.position);
            Vector3Int diagonalTopLeft = new(-1, -1);
            Vector3Int top = new(0, 1);
            Vector3Int diagonalTopRight = new(1, 1);
            Vector3Int left = new(1, 0);
            Vector3Int right = new(1, 0);
            Vector3Int diagonalBottomLeft = new(1, 1);
            Vector3Int bottom = new(0, 1);
            Vector3Int diagonalBottomRight = new(1, 1);

            return
                Tilemap!.HasTile(pos + diagonalTopLeft) ||
                Tilemap.HasTile(pos + top) ||
                Tilemap.HasTile(pos + diagonalTopRight) ||
                Tilemap.HasTile(pos + left) ||
                Tilemap.HasTile(pos + right) ||
                Tilemap.HasTile(pos + diagonalBottomLeft) ||
                Tilemap.HasTile(pos + bottom) ||
                Tilemap.HasTile(pos + diagonalBottomRight)
            ;
        }

        private void Update()
        {
            // Set selected bacterium per player input
            bool playerSelecting = Input.GetButtonDown(SelectButton);
            Vector3? mouseRayOrigin = null;
            if (playerSelecting) {
                mouseRayOrigin = Camera!.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hitInfo = Physics2D.GetRayIntersection(new Ray(mouseRayOrigin.Value, Vector3.forward), Mathf.Infinity, SelectLayerMask);
                if (hitInfo.collider != null && hitInfo.collider.TryGetComponent(out Bacterium bacterium))
                    setSelection(bacterium);
                else
                    setSelection(null);
            }

            // Set goal cell for selected bacterium, per player input
            bool playerSettingDest = Input.GetButtonDown(SetGoalButton);
            if (playerSettingDest && _selectedBacterium != null) {
                mouseRayOrigin ??= Camera!.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cell = Tilemap!.WorldToCell(mouseRayOrigin.Value);
                Goal goal = new(cell, Tilemap!.GetCellCenterWorld(cell));
                _bacteriaGoals[_selectedBacterium] = goal;
                if (_selectedBacterium.LineRenderer != null) {
                    _selectedBacterium.LineRenderer.enabled = true;
                    _selectedBacterium.LineRenderer.SetPosition(1, goal.WorldPosition);
                }
            }

            // Move all bacteria towards their goals
            _bacteriaStopped.Clear();
            foreach (Bacterium bacterium in _bacteriaGoals.Keys) {
                Goal goal = _bacteriaGoals[bacterium];
                Vector3 vectorToGoal = goal.WorldPosition - bacterium.transform.position;
                float distToGoal = vectorToGoal.magnitude;
                if (distToGoal <= MinOffsetFromGoal) {
                    bacterium.transform.position = goal.WorldPosition;
                    _bacteriaStopped.Add(bacterium);
                    //DestinationReached.Invoke((bacterium, goal.Cell));
                }
                else {
                    if (bacterium.LineRenderer != null)
                        bacterium.LineRenderer.SetPosition(0, bacterium.transform.position);

                    Vector3Int cell = Tilemap!.WorldToCell(bacterium.transform.position);
                    Vector3Int cellVectorToGoal = goal.Cell - cell;
                    Vector3 cellDirToGoal = new Vector3(cellVectorToGoal.x, cellVectorToGoal.y, cellVectorToGoal.z).normalized;
                    Vector3Int nextCell = new((int)(cell.x + cellDirToGoal.x), (int)(cell.y + cellDirToGoal.y));
                    if (!Tilemap!.HasTile(nextCell))
                        bacterium.transform.Translate(Speed * vectorToGoal / distToGoal);
                    else {
                        bacterium.transform.position = Tilemap!.GetCellCenterWorld(cell);
                        _bacteriaStopped.Add(bacterium);
                    }
                }
            }
            foreach (Bacterium bacterium in _bacteriaStopped) {   // Can't remove elements from the goal dictionary while enumerating it above
                _bacteriaGoals.Remove(bacterium);
                if (bacterium.LineRenderer != null)
                    bacterium.LineRenderer.enabled = false;
            }
        }
    }
}

