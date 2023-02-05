using UnityEngine;
using UnityEngine.Events;

namespace RootCanal
{
    public class Timer : MonoBehaviour
    {
        private float _currTime = 0f;

        public float IntervalSeconds = 0.25f;
        public UnityEvent Triggered = new();

        private void OnEnable() => _currTime = 0f;

        // Update is called once per frame
        private void Update()
        {
            _currTime += Time.deltaTime;
            if (_currTime >= IntervalSeconds) {
                _currTime -= IntervalSeconds;
                Triggered.Invoke();
            }
        }
    }
}
