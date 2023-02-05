using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RootCanal
{
    public class Mouthwash : MonoBehaviour
    {
        public Vector3 _target;
        public float speed = 5.0f;
        void Start()
        {
            _target = new Vector3(0, -75, 0);
            //hardcoded target value to be at the bottom of the screen
        }
        // Start is called before the first frame update
        void onEnable()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            var step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, _target, step);
            //if it reached the goal
            if(transform.position. y < _target.y)
            {
                Debug.Log("mouthwash reached the end. destroying it.");
                Destroy(this);

            }
        }
        void OnTriggerEnter2D(Collider2D col)
        {
            Debug.Log("GameObject1 collided with " + col.name);
            col.gameObject.GetComponent<Bacterium>().Health -= 2;
        }
    }
}
