using System.Linq;
using Packages.Animus.Unity.Runtime.Core;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Environment.PointOfInterest
{
    public class PointOfInterestRegistry : TypeRegistry<PointOfInterestRegistry, PointOfInterest>
    {
        public PointOfInterest GetRandomPoi()
        {
            return allItems.Count == 0 ? null : allItems[Random.Range(0, allItems.Count)];
        }

        public PointOfInterest GetNearestPoi(Vector3 position)
        {
            if (allItems.Count == 0)
            {
                return null;
            }

            return allItems.OrderBy(poi => Vector3.Distance(position, poi.transform.position))
                .FirstOrDefault();
        }
    }
}