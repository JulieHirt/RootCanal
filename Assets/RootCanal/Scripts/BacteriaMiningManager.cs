#nullable enable

using Sirenix.OdinInspector;
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
        public int DamagePerHit = 10;
        public int MinMoneyPerTile = 1;
        public int MaxMoneyPerTile = 3;

        private void Awake()
        {
            BacteriaManager!.BacteriumAdded.AddListener(bacterium => {
                Timer miningTimer = bacterium.GetComponent<Timer>();
                miningTimer.Triggered.AddListener(() => mine(bacterium));
            });

            TileInstanceManager!.TileInstanceCreated += onTileInstanceCreated;
        }

        private void onTileInstanceCreated(object sender, (TileInstance tileInstance, Vector3Int position) e) =>
            e.tileInstance.Durability!.AmountChanged.AddListener(delta => {
                if (e.tileInstance.Durability.CurrentAmount > 0)
                    return;

                Tilemap!.DeleteCells(e.position, new(1, 1, 1));
                TileInstanceManager!.BreakTileAt(e.position);
                int money = Random.Range(MinMoneyPerTile, MaxMoneyPerTile);
                MoneyContext!.AddToAmount(money);
            });

        private void mine(Bacterium bacterium)
        {
            TileInstance? tile = TileInstanceManager!.GetTileAtPosition(BacteriaMovementManager!.GoalTilePos);
            if (tile == null) {
                Debug.LogError($"Bacterium {bacterium.name} is mining a tile with no {nameof(TileInstance)}", context: bacterium);
                return;
            }

            Debug.Log($"Bacterium {bacterium.name} decreasing durability of tile instance {tile.name} by {DamagePerHit}...");
            tile.Durability!.AddToAmount(-DamagePerHit);
        }
    }
}
