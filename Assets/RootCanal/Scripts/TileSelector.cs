using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RootCanal
{
    public class TileSelector : MonoBehaviour
    {
        public Tilemap tm;//set this in the inspector. References the tilemap from the world prefab.
        TileBase selectedTile;

        // Update is called once per frame
        void Update()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(string.Format("Co-ords of mouse is [X: {0} Y: {0}]", mousePos.x, mousePos.y));
            Vector3Int coordinate = tm.WorldToCell(mousePos);
            Debug.Log(coordinate);
            transform.position = coordinate;
            transform.position += new Vector3(0.5f, 0.5f, 0);
            Debug.Log(transform.position);
            if (tm.HasTile(coordinate))
            {
                Debug.Log("there is a tile here!");
                selectedTile = tm.GetTile(coordinate);
                
            }

        }
    }
}
