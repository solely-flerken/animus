using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;

namespace Features.Goap.MoveTo
{
    public class MoveCapability : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("MoveCapability");

            builder.AddGoal<MoveGoal>()
                .AddCondition<IsAtLocation>(Comparison.GreaterThanOrEqual, 1);

            builder.AddAction<MoveAction>()
                .SetTarget<MoveTarget>()
                .SetMoveMode(ActionMoveMode.MoveBeforePerforming)
                .SetStoppingDistance(1f)
                .AddEffect<IsAtLocation>(EffectType.Increase);

            builder.AddTargetSensor<MoveTargetSensor>()
                .SetTarget<MoveTarget>();

            builder.AddWorldSensor<IsAtLocationSenor>()
                .SetKey<IsAtLocation>();

            return builder.Build();
        }
    }
}