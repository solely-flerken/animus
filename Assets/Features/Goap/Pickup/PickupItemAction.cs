using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using Packages.Animus.Unity.Runtime.Core.Entity;

namespace Features.Goap.Pickup
{
    public class PickupItemAction : GoapActionBase<PickupItemAction.Data>
    {
        public class Data : IActionData
        {
            public ITarget Target { get; set; }
            [GetComponent] public AnimusAgent AnimusAgent { get; set; }
            [GetComponent] public AgentPickupItemBehavior Behavior { get; set; }
        }

        public override void Start(IMonoAgent agent, Data data)
        {
        }

        public override IActionRunState Perform(IMonoAgent agent, Data data, IActionContext context)
        {
            var targetItem = data.Behavior?.targetItem;
            var inventory = data.AnimusAgent?.inventory;

            if (!targetItem || inventory == null)
            {
                return ActionRunState.Stop;
            }

            if (!inventory.AddItem(targetItem.itemData, targetItem.quantity))
            {
                return ActionRunState.Stop;
            }

            targetItem.Pickup();

            if (data.Behavior != null)
            {
                data.Behavior.targetItem = null;
            }

            return ActionRunState.Completed;
        }
    }
}