using Cysharp.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Modules.Agent.Actions
{
    [CreateAssetMenu(fileName = "PickUpItem", menuName = "Animus/NPC/Action/PickUpItem")]
    public class PickUpItemAction : NpcAction
    {
        [HideInInspector] public string itemKey;
        
        protected override UniTask<string> OnExecute(AnimusAgent agent)
        {
            var item = AnimusEntityRegistry.Instance.FindByGameKey<AnimusObject>(itemKey);

            if (item == null)
            {
                var text = $"Item not found: {itemKey}";
                Debug.Log(text);
                return new UniTask<string>(text);
            }

            agent.inventory.AddItem(item.itemData, item.quantity);
            item.Pickup();
            
            return new UniTask<string>("Successfully picked up.");
        }
    }
}