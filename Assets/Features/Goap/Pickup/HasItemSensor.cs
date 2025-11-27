using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Packages.Animus.Unity.Runtime.Core.Entity;

namespace Features.Goap.Pickup
{
    public class HasItemSensor : LocalWorldSensorBase
    {
        public override void Created()
        {
        }

        public override void Update()
        {
        }

        public override SenseValue Sense(IActionReceiver agent, IComponentReference references)
        {
            var animusAgent = references.GetCachedComponent<AnimusAgent>();
            var pickupBehavior = references.GetCachedComponent<AgentPickupItemBehavior>();

            if (!animusAgent || animusAgent.inventory == null || !pickupBehavior)
            {
                return false;
            }

            if (string.IsNullOrEmpty(pickupBehavior.targetItemTypeId))
            {
                return false;
            }

            var currentQuantity = animusAgent.inventory.GetItemQuantity(pickupBehavior.targetItemTypeId);
            return currentQuantity >= pickupBehavior.totalItemQuantityAfterPickup;
        }
    }
}