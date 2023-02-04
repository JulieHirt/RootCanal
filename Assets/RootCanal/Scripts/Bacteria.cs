using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RootCanal
{
    public class Bacteria : MonoBehaviour
    {
        Transform selectionSprite;
        SpriteRenderer m_SpriteRenderer;
        //TODO: give player the ability to upgrade this
        public float Speed = 10f;
        Vector2 lastClickedPos;
        Vector2 prevPos; //keep track of position in prev frame to determine if moving left or right
        bool moving;
        bool selected;//detects if the player has selected the bacteria to give commands to it

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
            if(Input.GetMouseButtonDown(0) && selected == true)
            {
                lastClickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                moving = true;
            }
            if(moving && (Vector2)transform.position != lastClickedPos)
            {
                float step = Speed*Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, lastClickedPos, step);
                //flip sprite left or right
                if(transform.position.x < prevPos.x)
                {
                    m_SpriteRenderer.flipX = false; //this is preferable to gameObject.GetComponent<Rigidbody2D>().transform.Rotate(0, 180, 0);
                    //this way does not flip the colider. and it results in bacteria not constantly flipping back and forth.
                }
                else if(transform.position.x > prevPos.x)
                {
                    m_SpriteRenderer.flipX = true;
                }
            }
            else
            {
                {
                    moving = false;
                }
            }
            prevPos = transform.position; //set the prevPos for the next update cycle
        
        }
    void OnMouseDown()//detect if the player has clicked on the bacteria to select it
    {//toggle if the bacteria is selected
        selected = !selected;
        if(selected)
        {
            selectionSprite.gameObject.SetActive(true);
        }
        else{
            selectionSprite.gameObject.SetActive(false);
        }
    }
    }
}

