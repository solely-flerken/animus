using System.Collections.Generic;
using Core.Events;
using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using Packages.Animus.Unity.Runtime.Modules.Agent;

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

        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context)
        {
            var text = data.Behavior?.text;
            var targetActor = data.Behavior?.targetActor;
            var animusAgent = data.AnimusAgent;

            if (string.IsNullOrEmpty(text) || targetActor == null || animusAgent == null || data.Behavior == null)
            {
                return ActionRunState.Stop;
            }

            animusAgent.conversationHistory.AddLine(new List<string> { animusAgent.gameKey, targetActor.gameKey }, animusAgent.gameKey, text);
            EventSystem.InvokeDisplayMessageInChat($"{animusAgent.name}: {text}");
            
            data.Behavior.hasFinishedTalking = true;

            return ActionRunState.Completed;
        }
    }
}