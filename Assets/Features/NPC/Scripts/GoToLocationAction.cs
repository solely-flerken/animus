using Cysharp.Threading.Tasks;
using Features.Goap.Agents;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Modules.Agent;
using Packages.Animus.Unity.Runtime.Modules.Agent.Actions;
using UnityEngine;

namespace Features.NPC.Scripts
{
    [CreateAssetMenu(fileName = "GoToLocationAction", menuName = "Animus/NPC/Action/GoToLocationAction")]
    public class GoToLocationAction : NpcAction
    {
        [HideInInspector] public string targetEntity;

        protected override UniTask<string> OnExecute(AnimusAgent animusAgent)
        {
            var brain = animusAgent.GetComponent<SimpleAgentBrain>();
            
            var poi = AnimusEntityRegistry.Instance.FindByGameKey<AnimusEntity>(targetEntity);
            if (poi == null)
            {
                Debug.Log("There is no Location named: " + targetEntity);
                return UniTask.FromResult($"There is no Location named: '{targetEntity}'");
            }

            Debug.Log($"Action: {actionKey} - Going to {poi.name}");

            brain.StartGoalMoveTo(poi);
            
            return UniTask.FromResult("success");
        }
    }
}