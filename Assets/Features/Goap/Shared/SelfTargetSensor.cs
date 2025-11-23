using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;

namespace Features.Goap.Shared
{
    public class SelfTargetSensor : LocalTargetSensorBase
    {
        public override void Created()
        {
        }

        public override void Update()
        {
        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            if (existingTarget != null && existingTarget.IsValid()) return existingTarget;

            return new TransformTarget(agent.Transform);
        }
    }
}