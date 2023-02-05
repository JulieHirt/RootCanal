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
            Debug.Log($"Existing bacterium {bacterium.name} added to manager");
            BacteriumAdded.Invoke(bacterium);
        }

        public void DivideBacterium(Bacterium bacterium)
        {
            GameObject newBacteriumObj = Instantiate(bacterium.gameObject, getDividedSpawnPosition(bacterium.transform.position), Quaternion.identity, bacterium.transform.parent);
            Bacterium newBacterium = newBacteriumObj.GetComponent<Bacterium>();
            _bacteria.Add(newBacterium);
            Debug.Log($"Bacterium {bacterium.name} divided to yield bacterium {newBacterium.name} (which was added to manager)");
            BacteriumAdded.Invoke(newBacterium);
        }

        public IEnumerator<Bacterium> GetEnumerator() => throw new System.NotImplementedException();

        public void KillBacterium(Bacterium bacterium)
        {
            _bacteria.Remove(bacterium);
            Debug.Log($"Killing bacterium {bacterium.name} and removing it from manager...");
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
