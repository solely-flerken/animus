using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Environment.PointOfInterest;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    [CreateAssetMenu(fileName = "GoToRandomPoiInstruction", menuName = "NPC/Instructions/GoToRandomPoiInstruction")]
    public class GoToRandomPoiInstruction : NpcInstruction
    {
        public override void Execute(Agent agent, Dictionary<string, object> payload)
        {
            var poi = PointOfInterestRegistry.Instance.GetRandomPoi();
            Debug.Log($"Instruction: {instructionKey} - Going to {poi.name}");
            agent.GoToPoi(poi);
        }
    }
}