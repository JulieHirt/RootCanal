#nullable enable

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RootCanal
{
    [Serializable]
    public class ToothTissueConfig
    {
        public string Name = "";
        public int MaxDurability = 100;
        public int DamagePerHit = 10;
        public Tile[] DamageTiles = Array.Empty<Tile>();
    }

    public class BacteriaMiningManager : MonoBehaviour
    {
        private readonly Dictionary<string, ToothTissueConfig> _toothTissueConfigByName = new();

        [Required] public Tilemap? Tilemap;
        [Required] public TileInstanceManager? TileInstanceManager;
        [Required] public BacteriaManager? BacteriaManager;
        [Required] public BacteriaMovementManager? BacteriaMovementManager;
        [Required] public QuantityContext? MoneyContext;

        [Header("Mining effects")]
        public ToothTissueConfig[] ToothTissueConfigs = Array.Empty<ToothTissueConfig>();

        [Header("Mining rewards")]
        public int MinMoneyPerTile = 1;
        public int MaxMoneyPerTile = 3;

        private void Awake()
        {
            foreach (ToothTissueConfig toothTissueConfig in ToothTissueConfigs)
                _toothTissueConfigByName[toothTissueConfig.Name] = toothTissueConfig;

            BacteriaManager!.BacteriumAdded.AddListener(bacterium =>
                bacterium.MiningTimer!.Triggered.AddListener(() => mine(bacterium)));

            TileInstanceManager!.TileInstanceCreated += onTileInstanceCreated;
        }

        private void onTileInstanceCreated(object sender, (TileInstance tileInstance, Vector3Int position) e)
        {
            TileBase tile = Tilemap!.GetTile(e.position);
            ToothTissueConfig toothTissueConfig = _toothTissueConfigByName[getTissueNameFromTileName(tile.name)];
            e.tileInstance.Durability!.MaxAmount = toothTissueConfig.MaxDurability;

            e.tileInstance.Durability!.AmountChanged.AddListener(delta =>
                onTileInstanceBroken(e.tileInstance, e.position));
        }

        private string getTissueNameFromTileName(string tileName) => tileName.Substring(0, tileName.IndexOf("_"));

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
            TileInstance? tileInstance = TileInstanceManager!.GetTileAtPosition(mineCell.Value);
            if (tileInstance == null) {
                Debug.LogError($"Bacterium {bacterium.name} is mining a cell with no {nameof(TileInstance)}", context: bacterium);
                return;
            }

            TileBase tile = Tilemap!.GetTile(mineCell.Value);
            ToothTissueConfig toothTissueConfig = _toothTissueConfigByName[getTissueNameFromTileName(tile.name)];
            Debug.Log($"Bacterium {bacterium.name} decreasing durability of tile instance {tileInstance.name} by {toothTissueConfig.DamagePerHit}...");
            tileInstance.Durability!.AddToAmount(-toothTissueConfig.DamagePerHit);
            if (tileInstance.Durability.CurrentAmount > tileInstance.Durability.MinAmount) {
                int damageTileIndex = (int)(tileInstance.Durability!.CurrentAmountFraction * toothTissueConfig.DamageTiles.Length - 1);
                Tilemap!.SetTile(mineCell.Value, toothTissueConfig.DamageTiles[damageTileIndex]);
            }
        }
    }
}
