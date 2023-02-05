using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace RootCanal
{
    public class Bacteria : MonoBehaviour
    {
        public Tilemap tm;//set this in the inspector. References the tilemap from the world prefab.
        Transform selectionSprite;
        SpriteRenderer m_SpriteRenderer;

        //TODO: give player the ability to upgrade this
        public float Speed = 10f;
        Vector2 lastClickedPos;
        Vector2 prevPos; //keep track of position in prev frame to determine if moving left or right
        public Vector3Int goalTilePos;
        bool moving;
        bool selected;//detects if the player has selected the bacteria to give commands to it
        public UnityEvent<Vector3Int> DestinationReached = new UnityEvent<Vector3Int>();
        void Start()
        {
            //Fetch the SpriteRenderer from the GameObject
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            //gets the selection sprite. assumes it is the first child.
            selectionSprite = this.gameObject.transform.GetChild(0);
            selectionSprite.gameObject.SetActive(false); //bacteria appearance is "unselected" at start
            prevPos = (Vector2)this.transform.position;

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0) && selected == true) {
                lastClickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                goalTilePos = tm.WorldToCell(lastClickedPos);
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
            }
            else {
                {
                    moving = false;
                    //raise an event
                    //todo:fix this so the position is where you sent it
                    //Debug.Log("reached destination" + goalTilePos);
                    DestinationReached.Invoke(goalTilePos);
                }
            }
            prevPos = transform.position; //set the prevPos for the next update cycle

        }
        void OnMouseDown()//detect if the player has clicked on the bacteria to select it
        {//toggle if the bacteria is selected
            selected = !selected;
            if (selected) {
                selectionSprite.gameObject.SetActive(true);
            }
            else {
                selectionSprite.gameObject.SetActive(false);
            }
        }
    }
}

