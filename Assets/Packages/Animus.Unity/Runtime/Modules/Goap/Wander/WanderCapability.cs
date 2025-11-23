using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;

namespace Packages.Animus.Unity.Runtime.Modules.Goap.Wander
{
    public class WanderCapability : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("WanderCapability");

            builder.AddGoal<WanderGoal>()
                .AddCondition<IsWandering>(Comparison.GreaterThanOrEqual, 1);

            builder.AddAction<WanderAction>()
                .SetTarget<WanderTarget>()
                .AddEffect<IsWandering>(EffectType.Increase);

            builder.AddTargetSensor<WanderTargetSensor>()
                .SetTarget<WanderTarget>();

            return builder.Build();
        }
    }
}