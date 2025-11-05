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
        
        public override void OnExecute(AnimusAgent animusAgent)
        {
            EventSystem.InvokeDisplayMessageInChat($"{animusAgent.gameKey}: {text}");
        }
    }
}