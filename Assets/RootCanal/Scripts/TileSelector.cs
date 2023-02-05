#nullable enable

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace RootCanal
{
    public class TileSelector : MonoBehaviour
    {
        [Required] public Tilemap? Tilemap;
        public bool Logging = false;
        public UnityEvent<Vector3Int> TileSelected = new();

        // Update is called once per frame
        private void Update()
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Logging)
                Debug.Log($"World coordinates of mouse are [X: {mouseWorldPos.x} Y: {mouseWorldPos.y}]");
            Vector3Int mouseCell = Tilemap!.WorldToCell(mouseWorldPos);
            if (Logging)
                Debug.Log($"Cell coordinates of mouse are [X: {mouseCell.x} Y: {mouseCell.y}]");

            transform.position = mouseCell;
            transform.position += new Vector3(0.5f, 0.5f, 0);

            if (Tilemap.HasTile(mouseCell)) {
                if (Logging)
                    Debug.Log("There is a tile here!");

                TileSelected.Invoke(mouseCell);
            }

        }
    }
}
