using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Environment;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    [CreateAssetMenu(fileName = "GoToRandomPoiAction", menuName = "Animus/NPC/Action/GoToRandomPoi")]
    public class GoToRandomPoiAction : NpcAction
    {
        public override void Execute(AnimusAgent animusAgent, Dictionary<string, object> payload)
        {
            var poi = AnimusEntityRegistry.Instance.GetRandom<PointOfInterest>();
            Debug.Log($"Action: {actionKey} - Going to {poi.name}");
            animusAgent.GoToPoi(poi);
        }
    }
}