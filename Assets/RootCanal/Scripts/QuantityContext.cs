#nullable enable

using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace RootCanal
{
    public class QuantityContext : MonoBehaviour
    {
        [field: ShowInInspector, SerializeField]
        public int CurrentAmount { get; private set; }

        [Min(0)] public int MaxAmount;
        [Min(0)] public int MinAmount;

        public UnityEvent<int> AmountChanged = new();

        public UnityEvent ReachedMinAmount = new();

        [Button]
        public void AddToAmount(int delta)
        {
            CurrentAmount = Math.Clamp(CurrentAmount + delta, MinAmount, MaxAmount);
            AmountChanged.Invoke(delta);
            Debug.Log("quantty context called");
            if(CurrentAmount <= MinAmount)
            {
                Debug.Log("reaced min amount in quantity context");
                ReachedMinAmount.Invoke();
            }
        }
    }
}
