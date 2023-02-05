using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace RootCanal
{
    public class Bacterium : MonoBehaviour
    {
        public bool Logging = false;
        public Tilemap Tilemap;//set this in the inspector. References the tilemap from the world prefab.
        private Transform selectionSprite;
        private SpriteRenderer m_SpriteRenderer;

        //TODO: give player the ability to upgrade this
        public float Speed = 10f;
        private Vector2 lastClickedPos;
        private Vector2 prevPos; //keep track of position in prev frame to determine if moving left or right
        public Vector3Int goalTilePos;
        private bool moving;
        private bool _isSelected;//detects if the player has selected the bacteria to give commands to it
        public UnityEvent<Vector3Int> DestinationReached = new UnityEvent<Vector3Int>();

        private void Start()
        {
            //Fetch the SpriteRenderer from the GameObject
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            //gets the selection sprite. assumes it is the first child.
            selectionSprite = this.gameObject.transform.GetChild(0);
            selectionSprite.gameObject.SetActive(false); //bacteria appearance is "unselected" at start
            prevPos = (Vector2)this.transform.position;

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
            Vector3Int diagonalTopLeft = new Vector3Int(pos.x-1, pos.y-1, pos.z);
            Vector3Int top = new Vector3Int(pos.x, pos.y+1, pos.z);
            Vector3Int diagonalTopRight = new Vector3Int(pos.x+1, pos.y+1, pos.z);
            Vector3Int left = new Vector3Int(pos.x-1, pos.y, pos.z);
            Vector3Int right = new Vector3Int(pos.x+1, pos.y, pos.z);
            Vector3Int diagonalBottomLeft = new Vector3Int(pos.x-1, pos.y-1, pos.z);
            Vector3Int bottom = new Vector3Int(pos.x, pos.y-1, pos.z);
            Vector3Int diagonalBottomRight = new Vector3Int(pos.x+1, pos.y-1, pos.z);

            if(Tilemap.HasTile(diagonalTopLeft) || Tilemap.HasTile(top) || Tilemap.HasTile(diagonalTopRight) || Tilemap.HasTile(left) || Tilemap.HasTile(right) || Tilemap.HasTile(diagonalBottomLeft) || Tilemap.HasTile(bottom) || Tilemap.HasTile(diagonalBottomRight))
            {
                return true;
            }
            else
            {
                return false;
            }   

        }

        // Update is called once per frame

        private void reachedDestination() //stop moving and raise event
        {
            moving = false;
            //move to center of cell
            Vector3Int cellPosition = Tilemap.LocalToCell(transform.localPosition);
        transform.localPosition = Tilemap.GetCellCenterLocal(cellPosition);
            //raise an event
            if (Logging)
            Debug.Log("reached destination" + goalTilePos);
            DestinationReached.Invoke(goalTilePos);
        }
        private void Update()
        {
            //nput.GetMouseButtonDown(0) detects a mouse click anywhere on the screen. Also
            if (Input.GetMouseButtonDown(0) && _isSelected == true)
             {
                lastClickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                goalTilePos = Tilemap.WorldToCell(lastClickedPos);
                moving = true;
            }

            if (moving && (Vector2)transform.position != lastClickedPos) {
                float step = Speed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, lastClickedPos, step);
                //flip sprite left or right
                if (transform.position.x < prevPos.x) {
                    m_SpriteRenderer.flipX = false; //this is preferable to gameObject.GetComponent<Rigidbody2D>().transform.Rotate(0, 180, 0);
                    //this way does not flip the colider. and it results in bacteria not constantly flipping back and forth.
                }
                else if (transform.position.x > prevPos.x) {
                    m_SpriteRenderer.flipX = true;
                }
                //to do: hey am I next to a tile? stop. (yes, even if I have not reached my destination!!)
                if(isAtLeastOneAdjacentTileMineable())
                {
                    reachedDestination();
                }
            }
            else {
                reachedDestination();
            }
            prevPos = transform.position; //set the prevPos for the next update cycle

        }
        void OnMouseDown()//detect if the player has clicked on the bacteria to select it
        //this function detects clicks on the bacteria sprite specifically
        //does not detect clicks on other areas of the screen
        {
        _isSelected = !_isSelected;//toggle if the bacteria is selected
        selectionSprite.gameObject.SetActive(_isSelected); //toggle the sprite that highlights when selected
        }
    }
}

