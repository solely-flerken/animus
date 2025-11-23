using Cysharp.Threading.Tasks;
using Features.Goap.Agents;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Modules.Agent;
using Packages.Animus.Unity.Runtime.Modules.Agent.Actions;
using UnityEngine;

namespace Features.NPC.Scripts
{
    [CreateAssetMenu(fileName = "PickUpItem", menuName = "Animus/NPC/Action/PickUpItem")]
    public class PickUpItemAction : NpcAction
    {
        [HideInInspector] public string itemKey;
        
        protected override UniTask<string> OnExecute(AnimusAgent agent)
        {
            var brain = agent.GetComponent<SimpleAgentBrain>();
            var item = AnimusEntityRegistry.Instance.FindByGameKey<AnimusObject>(itemKey);

            if (item == null)
            {
                var text = $"Item not found: {itemKey}";
                Debug.Log(text);
                return new UniTask<string>(text);
            }

            brain.StartGoalPickupItem(item);
            
            return new UniTask<string>("Successfully picked up.");
        }
    }
}