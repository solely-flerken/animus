using System.Collections.Generic;
using Core.Events;
using Cysharp.Threading.Tasks;
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
        
        protected override async UniTask<string> OnExecute(AnimusAgent animusAgent)
        {
            animusAgent.conversationHistory.AddLine(new List<string> { animusAgent.gameKey, targetActor.gameKey }, animusAgent.gameKey, text);
            EventSystem.InvokeDisplayMessageInChat($"{animusAgent.name}: {text}");
            await UniTask.Yield();
            return "Done talking.";
        }
    }
}