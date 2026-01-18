using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Core.Config.Script;
using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Modules.Environment
{
    public static class EnvironmentScanner
    {
        public static EnvironmentSnapshot CreateSnapshot(AnimusAgent animusAgent)
        {
            var snapshot = new EnvironmentSnapshot();

            var visibleObjects = new List<AnimusObject>();
            
            var nearbyObjects = AnimusGameManager.EntityRegistry.GetAll<AnimusObject>();
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
            
            snapshot.PointsOfInterest = AnimusGameManager.EntityRegistry.GetLocationsRelevantTo(animusAgent);

            return snapshot;
        }
    }
}