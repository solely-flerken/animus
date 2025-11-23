using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Packages.Animus.Unity.Runtime.Modules.Goap.Agents;

namespace Packages.Animus.Unity.Runtime.Modules.Goap.MoveTo
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