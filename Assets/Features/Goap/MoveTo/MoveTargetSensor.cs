using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Features.Goap.Agents;

namespace Features.Goap.MoveTo
{
    public class MoveTargetSensor : LocalTargetSensorBase
    {
        public override void Created()
        {
        }

        public override void Update()
        {
        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var brain = references.GetCachedComponent<SimpleAgentBrain>();

            if (!brain || brain.moveToPosition == null)
            {
                return null;
            }

            return new PositionTarget(brain.moveToPosition.Value);
        }
    }
}