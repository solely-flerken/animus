using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;

namespace Features.Goap.Pickup
{
    public class PickupItemCapability : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("PickupCapability");

            builder.AddGoal<PickupItemGoal>()
                .AddCondition<HasItem>(Comparison.GreaterThanOrEqual, 1);

            builder.AddAction<PickupItemAction>()
                .SetTarget<ItemTarget>()
                .SetStoppingDistance(2f)
                .AddEffect<HasItem>(EffectType.Increase);

            builder.AddTargetSensor<ItemTargetSensor>()
                .SetTarget<ItemTarget>();

            builder.AddWorldSensor<HasItemSensor>()
                .SetKey<HasItem>();

            return builder.Build();
        }
    }
}