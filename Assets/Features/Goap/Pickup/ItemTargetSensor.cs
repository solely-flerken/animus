using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;

namespace Features.Goap.Pickup
{
    public class ItemTargetSensor : LocalTargetSensorBase
    {
        public override void Created()
        {
        }

        public override void Update()
        {
        }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            var pickupBehavior = references.GetCachedComponent<AgentPickupItemBehavior>();

            if (!pickupBehavior || !pickupBehavior.targetItem)
            {
                return null;
            }
            
            return new PositionTarget(pickupBehavior.targetItem.transform.position);
        }
    }
}