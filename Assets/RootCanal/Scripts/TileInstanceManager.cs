#nullable enable

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RootCanal
{
    public class TileInstanceManager : MonoBehaviour
    {
        private readonly Dictionary<Vector3Int, TileInstance> _tiles = new();

        [Required] public Tilemap? Tilemap;
        [Required] public TileSelector? TileSelector;
        [Required] public BacteriaManager? BacteriaManager;
        public Transform? TileParent;
        [AssetsOnly] public GameObject? TileInstancePrefab;

        public event EventHandler<(TileInstance, Vector3Int)>? TileInstanceCreated;
        public event EventHandler<(TileInstance, Vector3Int)>? TileInstanceDestroyed;

        private void Awake() =>
            BacteriaManager!.BacteriumSpawned.AddListener(bacteria =>
                bacteria.DestinationReached.AddListener(onDestinationReached)
            );

        private void onDestinationReached(Vector3Int position)
        {
            if (_tiles.TryGetValue(position, out TileInstance tile))
                return;

            TileBase tileBase = Tilemap!.GetTile(position);
            GameObject tileObj = Instantiate(TileInstancePrefab, Tilemap.CellToWorld(position), Quaternion.identity, TileParent != null ? TileParent : transform)!;
            tile = tileObj.GetComponent<TileInstance>();
            if (tile == null)
                throw new Exception($"{nameof(TileInstancePrefab)} must have a {nameof(TileInstance)} component somewhere in its hierarchy");

            _tiles[position] = tile;

            TileInstanceCreated?.Invoke(this, (tile, position));
        }

        public void BreakTileAt(Vector3Int position)
        {
            if (!_tiles.TryGetValue(position, out TileInstance tile))
                return;

            _tiles.Remove(position);
            Destroy(tile);
            TileInstanceDestroyed?.Invoke(this, (tile, position));
        }

        public TileInstance? GetTileAtPosition(Vector3Int position) =>
            _tiles.TryGetValue(position, out TileInstance tileInstance) ? tileInstance : null;
    }
}
