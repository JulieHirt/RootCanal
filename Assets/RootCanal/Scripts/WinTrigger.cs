using UnityEngine;
using UnityEngine.Events;

namespace RootCanal
{
    public class WinTrigger : MonoBehaviour
    {
        public UnityEvent Won = new();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Bacterium bacterium)) {
                Won.Invoke();
            }
        }
    }
}
