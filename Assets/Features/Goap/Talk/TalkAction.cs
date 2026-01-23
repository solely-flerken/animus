using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using Cysharp.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Core.Entity;

namespace Features.Goap.Talk
{
    public class TalkAction : GoapActionBase<TalkAction.Data>
    {
        public class Data : IActionData
        {
            public ITarget Target { get; set; }
            [GetComponent] public AnimusAgent AnimusAgent { get; set; }
            [GetComponent] public TalkBehavior Behavior { get; set; }
        }
        
        public override void Start(IMonoAgent agent, Data data)
        {
            base.Start(agent, data);
            data.Behavior.hasStartedTalking = false;
        }
        
        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context)
        {
            var text = data.Behavior?.text;
            var targetActor = data.Behavior?.targetActor;
            var animusAgent = data.AnimusAgent;

            if (string.IsNullOrEmpty(text) || targetActor == null || animusAgent == null || data.Behavior == null)
            {
                return ActionRunState.Stop;
            }
            
            if (!data.Behavior.hasStartedTalking)
            {
                data.Behavior.hasStartedTalking = true;
                data.Behavior.TalkAsync(data.AnimusAgent.entityName).Forget();
            }

            if (!data.Behavior.hasFinishedTalking)
            {
                return ActionRunState.Continue;
            }

            return ActionRunState.Completed;
        }
        
        public override void End(IMonoAgent agent, Data data)
        {
            base.End(agent, data);
            data.Behavior.hasStartedTalking = false;
        }
    }
}