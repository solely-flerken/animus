using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Random = UnityEngine.Random;

namespace Features.Goap.Wander
{
    public class WanderAction : GoapActionBase<WanderAction.Data>
    {
        public class Data : IActionData
        {
            public ITarget Target { get; set; }
            public IActionRunState Timer { get; set; }
        }

        public override void Start(IMonoAgent agent, Data data)
        {
            var wait = Random.Range(1f, 5f);

            data.Timer = ActionRunState.Wait(wait);
        }

        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context)
        {
            if (data.Timer.IsRunning())
            {
                return data.Timer;
            }

            return ActionRunState.Completed;
        }
    }
}