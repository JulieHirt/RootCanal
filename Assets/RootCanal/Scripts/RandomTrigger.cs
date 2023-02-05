using System;
using UnityEngine;
using UnityEngine.Events;

namespace RootCanal
{
    public class RandomTrigger : MonoBehaviour
    {
        public UnityEvent[] Triggers = Array.Empty<UnityEvent>();

        public void Trigger() => Triggers[UnityEngine.Random.Range(0, Triggers.Length)].Invoke();
    }
}
