using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RootCanal
{
    public class BacteriaManager : MonoBehaviour, IEnumerable<Bacteria>
    {
        private readonly List<Bacteria> _bacteria = new();

        public UnityEvent<Bacteria> BacteriumSpawned = new();
        public UnityEvent<Bacteria> BacteriumKilled = new();

        public void DivideBacterium(Bacteria bacteria)
        {
            _bacteria.Add(bacteria);
            GameObject newBacteriaObj = Instantiate(bacteria.gameObject, getDividedSpawnPosition(bacteria.transform.position), Quaternion.identity, bacteria.transform.parent);
            Bacteria newBacteria = newBacteriaObj.GetComponent<Bacteria>();

            BacteriumSpawned.Invoke(newBacteria);
        }

        public IEnumerator<Bacteria> GetEnumerator() => throw new System.NotImplementedException();

        public void KillBacterium(Bacteria bacteria)
        {
            _bacteria.Remove(bacteria);
            BacteriumKilled.Invoke(bacteria);

            Destroy(bacteria.gameObject);
        }

        private Vector3 getDividedSpawnPosition(Vector3 originalPosition)
        {
            return originalPosition;
        }

        IEnumerator IEnumerable.GetEnumerator() => _bacteria.GetEnumerator();
    }
}
