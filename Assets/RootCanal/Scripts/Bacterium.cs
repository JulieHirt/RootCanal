#nullable enable

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace RootCanal
{
    public class Bacterium : MonoBehaviour
    {
        [Required] public SpriteRenderer? SpriteRenderer;
        [Required] public Timer? MiningTimer;
        [Required] public QuantityContext? HealthContext;
        public LineRenderer? LineRenderer;
        public UnityEvent Selected = new();
        public UnityEvent Deselected = new();
        public UnityEvent Idling = new();
        public UnityEvent Moving = new();
        public UnityEvent Mining = new();
    }
}

