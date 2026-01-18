using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Features.Goap.Agents;
using UnityEngine;

namespace Features.Goap.MoveTo
{
    public class IsAtLocationSensor : LocalWorldSensorBase
    {
        public override void Created()
        {
        }

        public override void Update()
        {
        }

        public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
        {
            var brain = references.GetCachedComponent<SimpleAgentBrain>();

            if (brain && brain.moveToPosition)
            {
                var distance = Vector3.Distance(agent.Transform.position, brain.moveToPosition.position);
                return distance <= 2f;
            }

            return false;
        }
    }
}