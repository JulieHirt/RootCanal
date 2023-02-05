using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RootCanal
{
    public class MouthwashSpawn : MonoBehaviour
    {
        public GameObject mouthwash;
        public void SpawnMouthwash()
        { 
            Instantiate(mouthwash, new Vector3(0, 52, 0), Quaternion.identity);
        }
    }
}
