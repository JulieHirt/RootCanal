#nullable enable

using Sirenix.OdinInspector;
using UnityEngine;

namespace RootCanal
{
    [CreateAssetMenu(fileName = "upgrade", menuName = nameof(RootCanal) + "/" + nameof(UpgradeAsset))]
    public class UpgradeAsset : ScriptableObject
    {
        [Required] public string Title = "";
        [Min(0)] public int Cost;
        [Required] public Sprite? Thumbnail;
    }
}
