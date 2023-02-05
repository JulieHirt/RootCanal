#nullable enable

using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RootCanal
{

    public class MenuUpgrades : MonoBehaviour
    {
        private UpgradeButton[]? _buttons;
        private int _selectedIndex = -1;

        [Required] public QuantityContext? MoneyContext;
        [Required] public BacteriaManager bacteriaManager;
        [Header("Money text")]
        [Required] public TMP_Text? TxtAmount;
        public string AmountFormatString = "{0}";
        public string OverflowString = "9999+";
        public int OverflowAmount = 10_000;

        [Header("Upgrades")]
        public int UpgradeButtonHeight = 160;
        public RectTransform? UpgradeButtonsParent;
        [Required, AssetsOnly] public GameObject? UpgradeButtonPrefab;
        [DisableInPlayMode] public UpgradeAsset[] Upgrades = Array.Empty<UpgradeAsset>();

        [Required] public Button? BtnBuy;

        private void Awake()
        {
            MoneyContext!.AmountChanged.AddListener(handleMoneyChange);
            BtnBuy!.onClick.AddListener(BuySelectedUpgrade);

            _buttons = new UpgradeButton[Upgrades.Length];
        }

        public void BuySelectedUpgrade()
        {
            MoneyContext!.AddToAmount(-Upgrades[_selectedIndex].Cost);
            
            if(Upgrades[_selectedIndex].Title == "Cell Divide")
            {
                Debug.Log("bought divide");
                //get one bacteria. the first one.
                Bacterium bac = FindObjectOfType<Bacterium>();
                bacteriaManager.DivideBacterium(bac);
            }
        }

        private void Start()
        {
            for (int x = 0; x < Upgrades.Length; x++) {
                UpgradeAsset upgrade = Upgrades[x];

                Transform parent = UpgradeButtonsParent != null ? UpgradeButtonsParent.transform : transform;
                GameObject btnObj = Instantiate(UpgradeButtonPrefab, parent)!;
                UpgradeButton btn = btnObj.GetComponent<UpgradeButton>();
                _buttons![x] = btn;

                int index = x;  // Ensure correct index is saved in Lambda during loop
                btn.Button!.onClick.AddListener(() => SelectUpgradeButton(index));

                Vector2 btnPos = btn.Root!.anchoredPosition;
                btn.Root.anchoredPosition = new Vector2(btnPos.x, x * -UpgradeButtonHeight);
                btn.ImgMain!.sprite = upgrade.Thumbnail;
            }

            handleMoneyChange(delta: 0);
            SelectUpgradeButton(buttonIndex: -1);
        }

        public void SelectUpgradeButton(int buttonIndex)
        {
            _selectedIndex = buttonIndex;
            for (int x = 0; x < Upgrades.Length; x++) {
                UpgradeButton btn = _buttons![x];
                btn.ImgSelected!.enabled = x == _selectedIndex;
            }

            BtnBuy!.interactable = _selectedIndex >= 0;

            if (buttonIndex >= 0)
                Debug.Log($"Upgrade button {_selectedIndex} (upgrade {Upgrades[_selectedIndex].Title}) selected");
            else
                Debug.Log($"Upgrade button deselected");
        }

        private void handleMoneyChange(int delta)
        {
            int currAmt = MoneyContext!.CurrentAmount;
            string amountStr = currAmt >= OverflowAmount ? OverflowString : string.Format(AmountFormatString, MoneyContext!.CurrentAmount);
            TxtAmount!.text = amountStr;

            for (int x = 0; x < Upgrades.Length; x++) {
                UpgradeAsset upgrade = Upgrades[x];
                UpgradeButton btn = _buttons![x];

                bool canAfford = currAmt >= upgrade.Cost;
                Debug.Log($"Can {(canAfford ? "" : "not")} afford upgrade {upgrade.Title} (cost {upgrade.Cost}) with money of {currAmt}");
                btn.Button!.interactable = canAfford;
                btn.ImgDisabled!.enabled = !canAfford;
            }
        }
    }
}
