#nullable enable

using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RootCanal
{
    public class BacteriaMiningManager : MonoBehaviour
    {
        [Required] public Tilemap? Tilemap;
        [Required] public TileInstanceManager? TileInstanceManager;
        [Required] public BacteriaManager? BacteriaManager;
        [Required] public BacteriaMovementManager? BacteriaMovementManager;
        [Required] public QuantityContext? MoneyContext;

        [Header("Mining effects")]
        public int DamagePerHit = 10;
        public Tile[] DamageTiles = Array.Empty<Tile>();

        [Header("Mining rewards")]
        public int MinMoneyPerTile = 1;
        public int MaxMoneyPerTile = 3;

        private void Awake()
        {
            BacteriaManager!.BacteriumAdded.AddListener(bacterium =>
                bacterium.MiningTimer!.Triggered.AddListener(() => mine(bacterium)));

            TileInstanceManager!.TileInstanceCreated += onTileInstanceCreated;
        }

        private void onTileInstanceCreated(object sender, (TileInstance tileInstance, Vector3Int position) e) =>
            e.tileInstance.Durability!.AmountChanged.AddListener(delta =>
                onTileInstanceBroken(e.tileInstance, e.position));

        private void onTileInstanceBroken(TileInstance tileInstance, Vector3Int position)
        {
            if (tileInstance.Durability!.CurrentAmount > 0)
                return;

            BacteriaMovementManager!.CeaseActionAt(position);
            TileInstanceManager!.BreakTileAt(position);
            int money = UnityEngine.Random.Range(MinMoneyPerTile, MaxMoneyPerTile);
            MoneyContext!.AddToAmount(money);
        }

        private void mine(Bacterium bacterium)
        {
            Vector3Int? mineCell = BacteriaMovementManager!.GetActionCell(bacterium);
            if (mineCell == null) {
                Debug.LogError($"Bacterium {bacterium.name} has no cell to mine", context: bacterium);
                return;
            }
            TileInstance? tile = TileInstanceManager!.GetTileAtPosition(mineCell.Value);
            if (tile == null) {
                Debug.LogError($"Bacterium {bacterium.name} is mining a cell with no {nameof(TileInstance)}", context: bacterium);
                return;
            }

            Debug.Log($"Bacterium {bacterium.name} decreasing durability of tile instance {tile.name} by {DamagePerHit}...");
            tile.Durability!.AddToAmount(-DamagePerHit);
            if (tile.Durability.CurrentAmount > tile.Durability.MinAmount) {
                int damageTileIndex = (int)(tile.Durability!.CurrentAmountFraction * DamageTiles.Length);
                Tilemap!.SetTile(mineCell.Value, DamageTiles[damageTileIndex]);
            }
        }
    }
}
