using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RootCanal
{
    public class TimerEvent : MonoBehaviour
    {
        public static event Action TimerAction;

        private void Update()
        {
            if (Timer._currTime == 0.0f)
            {
                TimerAction?.Invoke();
            }
        }
    }
}
