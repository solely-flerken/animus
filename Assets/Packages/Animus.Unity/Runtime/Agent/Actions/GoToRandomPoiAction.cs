using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Environment.PointOfInterest;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    [CreateAssetMenu(fileName = "GoToRandomPoiAction", menuName = "NPC/Actions/GoToRandomPoi")]
    public class GoToRandomPoiAction : NpcAction
    {
        public override void Execute(Agent agent, Dictionary<string, object> payload)
        {
            var poi = PointOfInterestRegistry.Instance.GetRandomPoi();
            Debug.Log($"Action: {actionKey} - Going to {poi.name}");
            agent.GoToPoi(poi);
        }
    }
}