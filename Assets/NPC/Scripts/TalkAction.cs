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
        public override void Execute(AnimusAgent animusAgent, Dictionary<string, object> payload)
        {
            EventSystem.InvokeDisplayMessageInChat($"{animusAgent.gameKey}: Hello!");
        }
    }
}