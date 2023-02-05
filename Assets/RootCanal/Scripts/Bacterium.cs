#nullable enable

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace RootCanal
{
    public class Bacterium : MonoBehaviour
    {
        [Required] public SpriteRenderer? SpriteRenderer;
        public UnityEvent Selected = new();
        public UnityEvent Deselected = new();
    }
}

