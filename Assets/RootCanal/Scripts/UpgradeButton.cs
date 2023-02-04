#nullable enable

using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RootCanal
{
    public class UpgradeButton : MonoBehaviour
    {
        [Required] public RectTransform? Root;
        [Required] public Button? Button;
        [Required] public TMP_Text? TxtTitle;
        [Required] public TMP_Text? TxtCost;
        [Required] public Image? ImgThumbnail;
        [Required] public Image? ImgDisabled;
    }
}
