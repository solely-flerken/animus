using System.Collections.Generic;
using System.Linq;
using Events;
using Packages.Animus.Unity.Runtime.Agent;
using Packages.Animus.Unity.Runtime.Agent.Actions;
using UnityEngine;

namespace NPC.Scripts
{
    [CreateAssetMenu(fileName = "TalkAction", menuName = "Animus/NPC/Action/Talk")]
    public class TalkAction : NpcAction
    {
        public override void Execute(AnimusAgent animusAgent, List<ActionPayloadParameter> payloadParameters)
        {
            var textValue = payloadParameters.First(p => p.name.Equals("text"));
            EventSystem.InvokeDisplayMessageInChat($"{animusAgent.gameKey}: {textValue.value}");
        }
    }
}