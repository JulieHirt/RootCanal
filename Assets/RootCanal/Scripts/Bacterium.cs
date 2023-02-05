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
        public int Health = 10;
        public LineRenderer? LineRenderer;
        public UnityEvent Selected = new();
        public UnityEvent Deselected = new();
        public UnityEvent Idling = new();
        public UnityEvent Moving = new();
        public UnityEvent Mining = new();

        public void Update()
        {
            if (Health <= 0)
            {
                Debug.Log("dying");
                Destroy(this.gameObject);
            }
        }
    }
}

