using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RootCanal
{
    public class BacteriaManager : MonoBehaviour, IEnumerable<Bacterium>
    {
        private readonly List<Bacterium> _bacteria = new();

        public UnityEvent<Bacterium> BacteriumAdded = new();
        public UnityEvent<Bacterium> BacteriumKilled = new();

        public void AddExistingBacterium(Bacterium bacterium)
        {
            _bacteria.Add(bacterium);
            BacteriumAdded.Invoke(bacterium);
        }

        public void DivideBacterium(Bacterium bacterium)
        {
            GameObject newBacteriumObj = Instantiate(bacterium.gameObject, getDividedSpawnPosition(bacterium.transform.position), Quaternion.identity, bacterium.transform.parent);
            Bacterium newBacterium = newBacteriumObj.GetComponent<Bacterium>();
            _bacteria.Add(newBacterium);
            BacteriumAdded.Invoke(newBacterium);
        }

        public IEnumerator<Bacterium> GetEnumerator() => throw new System.NotImplementedException();

        public void KillBacterium(Bacterium bacterium)
        {
            _bacteria.Remove(bacterium);
            BacteriumKilled.Invoke(bacterium);

            Destroy(bacterium.gameObject);
        }

        private Vector3 getDividedSpawnPosition(Vector3 originalPosition)
        {
            return originalPosition;
        }

        IEnumerator IEnumerable.GetEnumerator() => _bacteria.GetEnumerator();
    }
}
