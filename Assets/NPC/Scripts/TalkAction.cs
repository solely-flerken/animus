using System.Collections.Generic;
using Events;
using Packages.Animus.Unity.Runtime.Agent;
using Packages.Animus.Unity.Runtime.Agent.Actions;
using UnityEngine;

namespace NPC.Scripts
{
    [CreateAssetMenu(fileName = "TalkAction", menuName = "Animus/NPC/Action/Talk")]
    public class TalkAction : NpcAction
    {
        [HideInInspector]
        public string text;
        
        [HideInInspector]
        public string targetAgent;
        
        public override void OnExecute(AnimusAgent animusAgent)
        {
            animusAgent.conversationHistory.AddLine(new List<string> { animusAgent.gameKey, targetAgent }, animusAgent.gameKey, text);
            EventSystem.InvokeDisplayMessageInChat($"{animusAgent.name}: {text}");
        }
    }
}