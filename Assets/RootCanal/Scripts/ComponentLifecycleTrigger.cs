using UnityEngine;
using UnityEngine.Events;

namespace RootCanal
{
    public class ComponentLifecycleTrigger : MonoBehaviour
    {
        public UnityEvent AwakeTriggered = new();
        public UnityEvent StartTriggered = new();
        public UnityEvent OnEnableTriggered = new();
        public UnityEvent OnDisableTriggered = new();
        public UnityEvent OnDestroyTriggered = new();

        private void Awake() => AwakeTriggered.Invoke();
        private void Start() => StartTriggered.Invoke();
        private void OnEnable() => OnEnableTriggered.Invoke();
        private void OnDisable() => OnDisableTriggered.Invoke();
        private void OnDestroy() => OnDestroyTriggered.Invoke();
    }
}
