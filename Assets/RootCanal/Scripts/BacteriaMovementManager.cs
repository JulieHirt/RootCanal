#nullable enable

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace RootCanal
{
    public class BacteriaMovementManager : MonoBehaviour
    {
        private Vector2 _lastClickedPos;
        private Vector2 _prevPos; //keep track of position in prev frame to determine if moving left or right
        private bool _isMoving;
        private Bacterium? _selectedBacterium;//detects if the player has selected the bacteria to give commands to it

        [Required] public Tilemap? Tilemap;
        [Required] public BacteriaManager? BacteriaManager;
        [Required] public Camera? Camera;
        public string SelectButton = "Fire1";
        public LayerMask SelectLayerMask;
        [Min(0f)] public float Speed = 10f;   //TODO: give player the ability to upgrade this
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

        private Vector3Int getCurrentTilePos()
        {
            //use the tilemap and current position to determine the cordinates of the tile we are in.
            return Tilemap!.WorldToCell(transform.position);
        }
        private bool isAtLeastOneAdjacentTileMineable()
        {
            //get all the adjacent cells
            //check each cell to see if it has a "tile" in it (tile = minable piece of tooth)
            Vector3Int pos = getCurrentTilePos();
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

        private void reachedDestination() //stop moving and raise event
        {
            _isMoving = false;
            //move to center of cell
            Vector3Int cellPosition = Tilemap!.LocalToCell(transform.localPosition);
            transform.localPosition = Tilemap.GetCellCenterLocal(cellPosition);
            //raise an event
            if (Logging)
                Debug.Log("reached destination" + GoalTilePos);
            DestinationReached.Invoke((_selectedBacterium!, GoalTilePos));
        }

        private void Update()
        {
            bool playerSelecting = Input.GetButtonDown(SelectButton);
            if (playerSelecting) {
                Vector3 mouseRayOrigin = Camera!.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hitInfo = Physics2D.GetRayIntersection(new Ray(mouseRayOrigin, Vector3.forward), Mathf.Infinity, SelectLayerMask);
                if (hitInfo.collider != null) {
                    if (hitInfo.collider.TryGetComponent(out Bacterium bacterium)) {
                        setSelection(bacterium);
                        return;
                    }
                }
                setSelection(null);
            }

            ////Input.GetMouseButtonDown(1) detects a mouse click anywhere on the screen.
            //if (Input.GetMouseButtonDown(1) && _selectedBacterium) {
            //    _lastClickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //    GoalTilePos = Tilemap!.WorldToCell(_lastClickedPos);
            //    _isMoving = true;
            //}

            //if (_isMoving && (Vector2)transform.position != _lastClickedPos) {
            //    float step = Speed * Time.deltaTime;
            //    transform.position = Vector2.MoveTowards(transform.position, _lastClickedPos, step);
            //    //flip sprite left or right
            //    if (transform.position.x < _prevPos.x) {
            //        SpriteRenderer!.flipX = false; //this is preferable to gameObject.GetComponent<Rigidbody2D>().transform.Rotate(0, 180, 0);
            //        //this way does not flip the colider. and it results in bacteria not constantly flipping back and forth.
            //    }
            //    else if (transform.position.x > _prevPos.x) {
            //        SpriteRenderer!.flipX = true;
            //    }
            //    //to do: hey am I next to a tile? stop. (yes, even if I have not reached my destination!!)
            //    if (isAtLeastOneAdjacentTileMineable()) {
            //        reachedDestination();
            //    }
            //}
            //else {
            //    reachedDestination();
            //}
            //_prevPos = transform.position; //set the prevPos for the next update cycle
        }
    }
}

