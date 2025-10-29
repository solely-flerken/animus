using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Agent;
using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Environment
{
    public static class EnvironmentScanner
    {
        public static EnvironmentSnapshot CreateSnapshot(AnimusAgent animusAgent)
        {
            var snapshot = new EnvironmentSnapshot();

            var visibleObjects = new List<AnimusEntity>();
            
            var nearbyObjects = AnimusEntityRegistry.Instance.GetAll<AnimusEntity>();
            foreach (var @object in nearbyObjects)
            {
                var objTransform = @object.transform;
                var directionToObject = objTransform.position - animusAgent.transform.position;
                var distanceToObject = directionToObject.magnitude;

                // Is Object in range
                if (distanceToObject > animusAgent.perceptionRadius)
                    continue; 

                // Check if the object is within the NPCs field of view
                if (!(Vector3.Angle(animusAgent.transform.forward, directionToObject.normalized) < animusAgent.fieldOfViewAngle / 2))
                    continue;

                // If the raycast doesn't hit an obstacle, the object is visible.
                if (Physics.Raycast(animusAgent.transform.position, directionToObject.normalized, distanceToObject, animusAgent.obstacleLayer)) 
                    continue;
                
                // Don't detect itself
                if(animusAgent.gameKey.Equals(@object.gameKey))
                    continue;
                
                visibleObjects.Add(@object);
            }
            
            snapshot.VisibleObjects = visibleObjects;

            return snapshot;
        }
    }
}