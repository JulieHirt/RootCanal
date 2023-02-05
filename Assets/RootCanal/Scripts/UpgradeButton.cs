#nullable enable

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace RootCanal
{
    public class UpgradeButton : MonoBehaviour
    {
        [Required] public RectTransform? Root;
        [Required] public Button? Button;
        [Required] public Image? ImgMain;
        [Required] public Image? ImgDisabled;
        [Required] public Image? ImgSelected;
    }
}
