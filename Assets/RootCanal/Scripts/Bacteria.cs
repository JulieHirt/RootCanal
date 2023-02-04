using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RootCanal
{
    public class Bacteria : MonoBehaviour
    {
        //TODO: give player the ability to upgrade this
        public float Speed = 10f;
        Vector2 lastClickedPos;
        bool moving;
        bool selected;//detects if the player has selected the bacteria to give commands to it

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
            }
            else
            {
                {
                    moving = false;
                }
            }
        
        }
    void OnMouseDown()//detect if the player has clicked on the bacteria to select it
    {//toggle if the bacteria is selected
        selected = !selected;
    }
    }
}

