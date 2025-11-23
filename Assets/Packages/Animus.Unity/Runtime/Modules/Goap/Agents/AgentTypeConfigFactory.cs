using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Packages.Animus.Unity.Runtime.Modules.Goap.MoveTo;
using Packages.Animus.Unity.Runtime.Modules.Goap.Pickup;
using Packages.Animus.Unity.Runtime.Modules.Goap.Wander;

namespace Packages.Animus.Unity.Runtime.Modules.Goap.Agents
{
    public class AgentTypeConfigFactory : AgentTypeFactoryBase
    {
        public override IAgentTypeConfig Create()
        {
            var builder = new AgentTypeBuilder(AgentConstants.General);

            builder.AddCapability<WanderCapability>();
            builder.AddCapability<MoveCapability>();
            builder.AddCapability<PickupItemCapability>();
            
            return builder.Build();
        }
    }
}