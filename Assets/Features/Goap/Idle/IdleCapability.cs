using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Features.Goap.Shared;

namespace Features.Goap.Idle
{
    public class IdleCapability : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("IdleCapability");

            builder.AddGoal<IdleGoal>()
                .AddCondition<IsIdleComplete>(Comparison.GreaterThanOrEqual, 1);

            builder.AddAction<IdleAction>()
                .SetTarget<SelfTarget>()
                .AddEffect<IsIdleComplete>(EffectType.Increase)
                .SetValidateConditions(false);

            builder.AddTargetSensor<SelfTargetSensor>()
                .SetTarget<SelfTarget>();

            builder.AddWorldSensor<IdleSensor>()
                .SetKey<IsIdleComplete>();
            
            return builder.Build();
        }
    }
}