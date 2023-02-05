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
        private readonly HashSet<Bacterium> _bacteriaArrived = new();

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
                _bacteriaGoals[_selectedBacterium] = new Goal(cell, Tilemap!.GetCellCenterWorld(cell));
            }

            // Move all bacteria towards their goals
            _bacteriaArrived.Clear();
            foreach (Bacterium bacterium in _bacteriaGoals.Keys) {
                Goal goal = _bacteriaGoals[bacterium];
                Vector3 vectorToGoal = goal.WorldPosition - bacterium.transform.position;
                float distToGoal = vectorToGoal.magnitude;
                if (distToGoal <= MinOffsetFromGoal) {
                    bacterium.transform.position = goal.WorldPosition;
                    _bacteriaArrived.Add(bacterium); ;
                    //DestinationReached.Invoke((bacterium, goal.Cell));
                }
                else
                    bacterium.transform.Translate(Speed * vectorToGoal / distToGoal);
            }
            foreach (Bacterium bacterium in _bacteriaArrived)   // Can't remove elements from the goal dictionary while enumerating it above
                _bacteriaGoals.Remove(bacterium);
        }
    }
}

