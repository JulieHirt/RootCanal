#nullable enable

using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
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
            public bool HasTile;

            public Goal(Vector3Int cell, Vector3 worldPosition, bool hasTile)
            {
                Cell = cell;
                WorldPosition = worldPosition;
                HasTile = hasTile;
            }
        }

        private Bacterium? _selectedBacterium;
        private readonly Dictionary<Bacterium, Goal> _bacteriaGoals = new();
        private readonly Dictionary<Bacterium, Vector3Int> _bacteriaActionCells = new();
        private readonly HashSet<Bacterium> _bacteriaStopped = new();

        [Required] public Tilemap? Tilemap;
        [Required] public BacteriaManager? BacteriaManager;
        [Required] public Camera? Camera;
        public bool Logging = false;

        [Header("Selecting")]
        public string SelectButton = "Fire1";

        [Header("Goal Setting")]
        public LayerMask SelectLayerMask;
        public string SetGoalButton = "Fire2";
        [Min(0f)] public float Speed = 0.1f;   //TODO: give player the ability to upgrade this
        [Min(0.001f)] public float MinOffsetFromGoal = 0.05f;
        [Required] public Tile? EmptyTile;
        public UnityEvent<(Bacterium, Vector3Int)> GoalReached = new();
        public UnityEvent<(Bacterium, Vector3Int)> CanActionTile = new();

        public Vector3Int? GetActionCell(Bacterium bacterium) =>
            _bacteriaActionCells.TryGetValue(bacterium, out Vector3Int cell) ? cell : null;

        public void CeaseActionAt(Vector3Int position)
        {
            Bacterium[] actioningBacteria = _bacteriaActionCells.Where(x => x.Value == position).Select(x => x.Key).ToArray();
            foreach (Bacterium bacterium in actioningBacteria) {
                _bacteriaActionCells.Remove(bacterium);
                bacterium.Idling.Invoke();
            }
        }

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
                TileBase tile = Tilemap!.GetTile(cell);
                bool hasTile = tile != null && tile.name != EmptyTile!.name;
                Goal goal = new(cell, Tilemap!.GetCellCenterWorld(cell), hasTile);
                Debug.Log($"Bacterium {_selectedBacterium.name}'s goal cell set to {goal.Cell}");
                _bacteriaGoals[_selectedBacterium] = goal;
                _bacteriaActionCells.Remove(_selectedBacterium);
                if (_selectedBacterium.LineRenderer != null) {
                    _selectedBacterium.LineRenderer.enabled = true;
                    _selectedBacterium.LineRenderer.SetPosition(1, goal.WorldPosition);
                }
                _selectedBacterium.Moving.Invoke();
            }

            // Move all bacteria towards their goals
            _bacteriaStopped.Clear();
            foreach (Bacterium bacterium in _bacteriaGoals.Keys) {
                Goal goal = _bacteriaGoals[bacterium];
                Vector3 vectorToGoal = goal.WorldPosition - bacterium.transform.position;
                float distToGoal = vectorToGoal.magnitude;
                if (distToGoal <= MinOffsetFromGoal) {
                    Debug.Log($"Bacterium {bacterium.name} reached its goal cell {goal.Cell}");
                    bacterium.transform.position = goal.WorldPosition;
                    _bacteriaStopped.Add(bacterium);
                    bacterium.Idling.Invoke();
                    GoalReached.Invoke((bacterium, goal.Cell));
                }
                else {
                    if (bacterium.LineRenderer != null)
                        bacterium.LineRenderer.SetPosition(0, bacterium.transform.position);

                    Vector3Int cell = Tilemap!.WorldToCell(bacterium.transform.position);
                    Vector3Int cellVectorToGoal = goal.Cell - cell;
                    Vector3 cellDirToGoal = new Vector3(cellVectorToGoal.x, cellVectorToGoal.y, cellVectorToGoal.z).normalized;
                    Vector3Int nextCell = new((int)(cell.x + cellDirToGoal.x), (int)(cell.y + cellDirToGoal.y));
                    TileBase nextTile = Tilemap!.GetTile(nextCell);
                    if (nextTile == null || nextTile.name == EmptyTile!.name)
                        bacterium.transform.Translate(Speed * vectorToGoal / distToGoal);
                    else {
                        bacterium.transform.position = Tilemap!.GetCellCenterWorld(cell);
                        _bacteriaStopped.Add(bacterium);
                        if (goal.HasTile && Mathf.Abs(goal.Cell.x - cell.x) <= 1 && Mathf.Abs(goal.Cell.y - cell.y) <= 1) {
                            Debug.Log($"Bacterium {bacterium.name} reached cell {cell} and is now actioning its goal cell {goal.Cell}");
                            _bacteriaActionCells[bacterium] = goal.Cell;
                            bacterium.Mining.Invoke();
                            CanActionTile.Invoke((bacterium, goal.Cell));
                        }
                        else {
                            Debug.Log($"Bacterium {bacterium.name} was stopped at cell {cell} on its way to goal cell {goal.Cell}");
                            bacterium.Idling.Invoke();
                        }
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

