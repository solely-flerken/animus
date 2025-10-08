using System.Collections.Generic;
using Events;
using Packages.Animus.Unity.Runtime.Agent;
using Packages.Animus.Unity.Runtime.Agent.Actions;
using UnityEngine;

namespace NPC.Scripts
{
    [CreateAssetMenu(fileName = "TalkInstruction", menuName = "NPC/Instructions/TalkInstruction")]
    public class TalkInstruction : NpcInstruction
    {
        public override void Execute(Agent agent, Dictionary<string, object> payload)
        {
            EventSystem.InvokeDisplayMessageInChat($"{agent.agentModel.gameKey}: Hello!");
        }
    }
}