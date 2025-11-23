using Cysharp.Threading.Tasks;
using Features.Goap.Agents;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Modules.Agent;
using Packages.Animus.Unity.Runtime.Modules.Agent.Actions;
using UnityEngine;

namespace Features.NPC.Scripts
{
    [CreateAssetMenu(fileName = "TalkAction", menuName = "Animus/NPC/Action/Talk")]
    public class TalkAction : NpcAction
    {
        [HideInInspector]
        public string text;
        
        [HideInInspector]
        public AnimusActor targetActor;
        
        protected override UniTask<string> OnExecute(AnimusAgent animusAgent)
        {
            var brain = animusAgent.GetComponent<SimpleAgentBrain>();

            if (brain == null)
            {
                return UniTask.FromResult("failure");
            }

            brain.StartGoalTalk(text, targetActor);
            
            return UniTask.FromResult("success");
        }
    }
}