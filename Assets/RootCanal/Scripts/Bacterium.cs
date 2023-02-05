#nullable enable

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace RootCanal
{
    public class Bacterium : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private Vector2 _lastClickedPos;
        private Vector2 _prevPos; //keep track of position in prev frame to determine if moving left or right
        private bool _isMoving;
        private bool _isSelected;//detects if the player has selected the bacteria to give commands to it

        [Required] public Tilemap? Tilemap;//set this in the inspector. References the tilemap from the world prefab.
        [Min(0f)] public float Speed = 10f;   //TODO: give player the ability to upgrade this
        public Vector3Int GoalTilePos;
        public bool Logging = false;
        public Transform? SelectionObject;
        public UnityEvent<Vector3Int> DestinationReached = new();

        private void Awake()
        {
            //Fetch the SpriteRenderer from the GameObject
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            if (SelectionObject != null)
                SelectionObject.gameObject.SetActive(false); //bacteria appearance is "unselected" at start
            _prevPos = (Vector2)this.transform.position;
        }

        private Vector3Int getCurrentTilePos()
        {
            //use the tilemap and current position to determine the cordinates of the tile we are in.
            return Tilemap.WorldToCell(transform.position);
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
            DestinationReached.Invoke(GoalTilePos);
        }
        private void Update()
        {
            //Input.GetMouseButtonDown(1) detects a mouse click anywhere on the screen.
            if (Input.GetMouseButtonDown(1) && _isSelected == true) {
                _lastClickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                GoalTilePos = Tilemap!.WorldToCell(_lastClickedPos);
                _isMoving = true;
            }

            if (_isMoving && (Vector2)transform.position != _lastClickedPos) {
                float step = Speed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, _lastClickedPos, step);
                //flip sprite left or right
                if (transform.position.x < _prevPos.x) {
                    _spriteRenderer.flipX = false; //this is preferable to gameObject.GetComponent<Rigidbody2D>().transform.Rotate(0, 180, 0);
                    //this way does not flip the colider. and it results in bacteria not constantly flipping back and forth.
                }
                else if (transform.position.x > _prevPos.x) {
                    _spriteRenderer.flipX = true;
                }
                //to do: hey am I next to a tile? stop. (yes, even if I have not reached my destination!!)
                if (isAtLeastOneAdjacentTileMineable()) {
                    reachedDestination();
                }
            }
            else {
                reachedDestination();
            }
            _prevPos = transform.position; //set the prevPos for the next update cycle

        }

        /// <summary>
        /// detect if the player has clicked on the bacteria to select it
        /// this function detects clicks on the bacteria sprite specifically
        /// does not detect clicks on other areas of the screen
        /// </summary>
        private void OnMouseDown()
        {
            _isSelected = !_isSelected;//toggle if the bacteria is selected
            if (SelectionObject != null)
                SelectionObject.gameObject.SetActive(_isSelected); //toggle the sprite that highlights when selected
        }
    }
}

