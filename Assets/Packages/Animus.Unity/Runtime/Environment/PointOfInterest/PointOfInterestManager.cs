using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Environment.PointOfInterest
{
    public class PointOfInterestManager : MonoBehaviour
    {
        public static PointOfInterestManager Instance { get; private set; }

        private readonly List<PointOfInterest> _allPointsOfInterest = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void RegisterPoi(PointOfInterest poi)
        {
            if (!_allPointsOfInterest.Contains(poi))
            {
                _allPointsOfInterest.Add(poi);
            }
        }

        public void UnregisterPoi(PointOfInterest poi)
        {
            if (_allPointsOfInterest.Contains(poi))
            {
                _allPointsOfInterest.Remove(poi);
            }
        }

        public PointOfInterest GetRandomPoi()
        {
            if (_allPointsOfInterest.Count == 0)
            {
                return null;
            }

            return _allPointsOfInterest[Random.Range(0, _allPointsOfInterest.Count)];
        }

        public PointOfInterest GetNearestPoi(Vector3 position)
        {
            if (_allPointsOfInterest.Count == 0)
            {
                return null;
            }

            return _allPointsOfInterest.OrderBy(poi => Vector3.Distance(position, poi.transform.position))
                .FirstOrDefault();
        }
    }
}