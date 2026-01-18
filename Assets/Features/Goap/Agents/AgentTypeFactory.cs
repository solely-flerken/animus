using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Features.Goap.Idle;
using Features.Goap.MoveTo;
using Features.Goap.Pickup;
using Features.Goap.Talk;
using Features.Goap.Wander;

namespace Features.Goap.Agents
{
    public class AgentTypeFactory : AgentTypeFactoryBase
    {
        public override IAgentTypeConfig Create()
        {
            var builder = CreateBuilder(AgentTypes.General);
            
            builder.AddCapability<WanderCapability>();
            builder.AddCapability<MoveCapability>();
            builder.AddCapability<PickupItemCapability>();
            builder.AddCapability<TalkCapability>();
            builder.AddCapability<IdleCapability>();
            
            return builder.Build();
        }
    }
}