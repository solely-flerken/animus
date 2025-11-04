using System.Collections.Generic;
using System.Linq;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Environment;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    [CreateAssetMenu(fileName = "GoToRandomPoiAction", menuName = "Animus/NPC/Action/GoToRandomPoi")]
    public class GoToRandomPoiAction : NpcAction
    {
        public override void Execute(AnimusAgent animusAgent, List<ActionPayloadParameter> payloadParameters)
        {
            var poiKey = payloadParameters.First(p => p.name.Equals("locationKey"));
            var poi = AnimusEntityRegistry.Instance.FindByGameKey<AnimusLocation>(poiKey.value);
            Debug.Log($"Action: {actionKey} - Going to {poi.name}");
            animusAgent.GoToPoi(poi);
        }
    }
}