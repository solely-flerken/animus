using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace Features.Goap.Idle
{
    public class IdleAction : GoapActionBase<IdleAction.Data>
    {
        public class Data : IActionData
        {
            public ITarget Target { get; set; }
            public float Timer { get; set; }
        }

        public override void Start(IMonoAgent agent, Data data)
        {
            data.Timer = Random.Range(7f, 23f);
        }

        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context)
        {
            data.Timer -= context.DeltaTime;
            
            if (data.Timer <= 0)
            {
                // Debug.Log("Next idle phase (new Idle animation).");
                data.Timer = Random.Range(7f, 23f);
            }
            
            return ActionRunState.ContinueOrResolve;
        }
    }
}