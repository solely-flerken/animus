using System.Linq;
using Packages.Animus.Unity.Runtime.Core;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Environment.PointOfInterest
{
    public class PointOfInterestRegistry : TypeRegistry<PointOfInterestRegistry, PointOfInterest>
    {
        public PointOfInterest GetRandomPoi()
        {
            if (AllItems.Count == 0)
            {
                return null;
            }

            return AllItems[Random.Range(0, AllItems.Count)];
        }

        public PointOfInterest GetNearestPoi(Vector3 position)
        {
            if (AllItems.Count == 0)
            {
                return null;
            }

            return AllItems.OrderBy(poi => Vector3.Distance(position, poi.transform.position))
                .FirstOrDefault();
        }
    }
}