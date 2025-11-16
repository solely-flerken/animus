using System.Collections.Generic;
using Core.Events;
using Packages.Animus.Unity.Runtime.Agent;
using Packages.Animus.Unity.Runtime.Agent.Actions;
using Packages.Animus.Unity.Runtime.Core.Entity;
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
        
        public override void OnExecute(AnimusAgent animusAgent)
        {
            animusAgent.conversationHistory.AddLine(new List<string> { animusAgent.gameKey, targetActor.gameKey }, animusAgent.gameKey, text);
            EventSystem.InvokeDisplayMessageInChat($"{animusAgent.name}: {text}");
        }
    }
}