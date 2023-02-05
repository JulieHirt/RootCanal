using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RootCanal
{
    public class BacteriaHealthManager : MonoBehaviour
    {
        [Required] public BacteriaManager? BacteriaManager; //required is an Odin thing

        private void Awake()
        {
            BacteriaManager!.BacteriumAdded.AddListener(bacterium => {
                bacterium.GetComponent<QuantityContext>().ReachedMinAmount.AddListener(() => die(bacterium));
            });

        }

        private void die(Bacterium b)
        {
            BacteriaManager!.KillBacterium(b);
            //play a sound or something?
        }
    }
}
