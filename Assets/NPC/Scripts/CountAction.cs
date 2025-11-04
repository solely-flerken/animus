using System.Collections.Generic;
using System.Threading.Tasks;
using Events;
using Packages.Animus.Unity.Runtime.Agent;
using Packages.Animus.Unity.Runtime.Agent.Actions;
using UnityEngine;

namespace NPC.Scripts
{
    [CreateAssetMenu(fileName = "CountAction", menuName = "Animus/NPC/Action/Count")]
    public class CountAction : NpcAction
    {
        public override async void Execute(AnimusAgent animusAgent, List<ActionPayloadParameter> payloadParameters)
        {
            for (var i = 1; i <= 10; i++)
            {
                EventSystem.InvokeDisplayMessageInChat($"{animusAgent.gameKey}: {i.ToString()}");
                await Task.Delay(1000);
            }
        }
    }
}