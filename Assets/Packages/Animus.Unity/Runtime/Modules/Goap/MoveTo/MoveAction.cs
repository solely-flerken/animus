using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;

namespace Packages.Animus.Unity.Runtime.Modules.Goap.MoveTo
{
    public class MoveAction : GoapActionBase<MoveAction.Data>
    {
        public class Data : IActionData
        {
            public ITarget Target { get; set; }
        }

        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context)
        {
            return ActionRunState.Completed;
        }
    }
}