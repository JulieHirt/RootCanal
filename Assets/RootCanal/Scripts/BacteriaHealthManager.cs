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
            Debug.Log("added lister");
            BacteriaManager!.BacteriumAdded.AddListener(bacterium => {
                Debug.Log("reall setgin t up thelister");
                bacterium.HealthContext!.ReachedMinAmount.AddListener(() => die(bacterium));
            });

        }

        private void die(Bacterium b)
        {
            Debug.Log("bacteria died");
            BacteriaManager!.KillBacterium(b);
            //play a sound or something?
        }
    }
}
