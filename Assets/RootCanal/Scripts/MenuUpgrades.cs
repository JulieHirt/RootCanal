#nullable enable

using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;

namespace RootCanal
{

    public class MenuUpgrades : MonoBehaviour
    {
        private UpgradeButton[] _buttons;

        [Required] public MoneyContext? MoneyContext;

        [Header("Money text")]
        [Required] public TMP_Text? TxtAmount;
        public string AmountFormatString = "{0}";
        public string OverflowString = "9999+";
        public int OverflowAmount = 10_000;

        [Header("Upgrades")]
        public int UpgradeButtonHeight = 160;
        public RectTransform UpgradeButtonsParent;
        public string CostFormatString = "{0}";
        [Required, AssetsOnly] public GameObject UpgradeButtonPrefab;
        [DisableInPlayMode] public UpgradeAsset[] Upgrades = Array.Empty<UpgradeAsset>();

        private void Awake()
        {
            MoneyContext!.AmountChanged.AddListener(handleMoneyChange);

            _buttons = new UpgradeButton[Upgrades.Length];
        }

        private void Start()
        {
            for (int x = 0; x < Upgrades.Length; x++) {
                UpgradeAsset upgrade = Upgrades[x];

                Transform parent = UpgradeButtonsParent != null ? UpgradeButtonsParent.transform : transform;
                GameObject btnObj = Instantiate(UpgradeButtonPrefab, parent);
                UpgradeButton btn = btnObj.GetComponent<UpgradeButton>();
                _buttons[x] = btn;

                Vector2 btnPos = btn.Root.anchoredPosition;
                btn.Root.anchoredPosition = new Vector2(btnPos.x, x * -UpgradeButtonHeight);
                btn.TxtTitle.text = upgrade.Title;
                btn.TxtCost.text = string.Format(CostFormatString, upgrade.Cost);
                btn.ImgThumbnail.sprite = upgrade.Thumbnail;
            }

            handleMoneyChange(delta: 0);
        }

        private void handleMoneyChange(int delta)
        {
            int currAmt = MoneyContext!.CurrentAmount;
            string amountStr = currAmt >= OverflowAmount ? OverflowString : string.Format(AmountFormatString, MoneyContext!.CurrentAmount);
            TxtAmount!.text = amountStr;

            for (int x = 0; x < Upgrades.Length; x++) {
                UpgradeAsset upgrade = Upgrades[x];
                UpgradeButton btn = _buttons[x];

                bool canAfford = currAmt >= upgrade.Cost;
                btn.enabled = canAfford;
                btn.ImgDisabled.enabled = !canAfford;
            }
        }
    }
}
