using System.Threading.Tasks;
using Core.Events;
using Cysharp.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Agent;
using Packages.Animus.Unity.Runtime.Agent.Actions;
using UnityEngine;

namespace Features.NPC.Scripts
{
    [CreateAssetMenu(fileName = "CountAction", menuName = "Animus/NPC/Action/Count")]
    public class CountAction : NpcAction
    {
        protected override async UniTask<string> OnExecute(AnimusAgent animusAgent)
        {
            for (var i = 1; i <= 10; i++)
            {
                EventSystem.InvokeDisplayMessageInChat($"{animusAgent.gameKey}: {i.ToString()}");
                await Task.Delay(1000);
            }

            return "Done counting.";
        }
    }
}