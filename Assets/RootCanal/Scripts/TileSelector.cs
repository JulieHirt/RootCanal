using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RootCanal
{
    public class TileSelector : MonoBehaviour
    {
        Tilemap tm;
        // Start is called before the first frame update
        void Start()
        {
        tm = GetComponent<Tilemap>();//get a reference to the tilemap
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(string.Format("Co-ords of mouse is [X: {0} Y: {0}]", mousePos.x, mousePos.y));
            /*TileData tile = world.Tile((int)pos.x, (int)pos.y);
 
            if (tile != null)
            {
                Debug.Log(string.Format("Tile is: {0}", tile.TileType));
            }*/
            Vector3Int coordinate = tm.WorldToCell(mousePos);
            Debug.Log(coordinate);

        }
    }
}
