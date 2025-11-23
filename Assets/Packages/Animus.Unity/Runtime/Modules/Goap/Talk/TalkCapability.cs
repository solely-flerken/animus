using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Packages.Animus.Unity.Runtime.Modules.Goap.Shared;

namespace Packages.Animus.Unity.Runtime.Modules.Goap.Talk
{
    public class TalkCapability : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("TalkCapability");

            builder.AddGoal<TalkGoal>()
                .AddCondition<HasTalked>(Comparison.GreaterThanOrEqual, 1);

            builder.AddAction<TalkAction>()
                .SetTarget<SelfTarget>()
                .AddEffect<HasTalked>(EffectType.Increase);

            builder.AddTargetSensor<SelfTargetSensor>()
                .SetTarget<SelfTarget>();
            
            builder.AddWorldSensor<HasTalkedSensor>()
                .SetKey<HasTalked>();
            
            return builder.Build();
        }
    }
}