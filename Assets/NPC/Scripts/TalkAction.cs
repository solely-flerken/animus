using System.Collections.Generic;
using Events;
using Packages.Animus.Unity.Runtime.Agent;
using Packages.Animus.Unity.Runtime.Agent.Actions;
using UnityEngine;

namespace NPC.Scripts
{
    [CreateAssetMenu(fileName = "TalkAction", menuName = "NPC/Action/Talk")]
    public class TalkAction : NpcAction
    {
        public override void Execute(Agent agent, Dictionary<string, object> payload)
        {
            EventSystem.InvokeDisplayMessageInChat($"{agent.animusAgent.gameKey}: Hello!");
        }
    }
}