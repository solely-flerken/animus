using System.Linq;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Modules.Agent;
using Packages.Animus.Unity.Runtime.Modules.Goap.MoveTo;
using Packages.Animus.Unity.Runtime.Modules.Goap.Pickup;
using Packages.Animus.Unity.Runtime.Modules.Goap.Wander;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Modules.Goap.Agents
{
    public class SimpleAgentBrain : MonoBehaviour
    {
        private GoapBehaviour _goapBehaviour;
        private GoapActionProvider _provider;

        private AgentPickupItemBehavior _pickupItemBehavior;
        private AnimusAgent _animusAgent;

        public Vector3? moveToPosition;

        private void Awake()
        {
            _goapBehaviour = FindObjectsByType<GoapBehaviour>(FindObjectsSortMode.None).First();

            _provider = GetComponent<GoapActionProvider>();
            _provider.AgentType = _goapBehaviour.GetAgentType(AgentConstants.General);

            _pickupItemBehavior = GetComponent<AgentPickupItemBehavior>();
            _animusAgent = GetComponent<AnimusAgent>();
        }

        private void OnEnable()
        {
            _provider.Events.OnGoalCompleted += OnGoalCompleted;
        }

        private void OnDisable()
        {
            _provider.Events.OnGoalCompleted -= OnGoalCompleted;
        }

        private void OnGoalCompleted(IGoal goal)
        {
            if (goal is MoveGoal)
            {
                moveToPosition = null;
                _provider.RequestGoal<WanderGoal>();
            }
        }

        private void Start()
        {
            // TODO: Fix Agent tries to achieve the goal after it was completed. Spams: "Trying to resolve goals" -> "No action found for goals" 
            var item = AnimusEntityRegistry.Instance.GetRandom<AnimusObject>();
            _provider.RequestGoal<PickupItemGoal>();
            _pickupItemBehavior.targetItemTypeId = item.itemData.itemTypeId;
            _pickupItemBehavior.targetItem = item;
            _pickupItemBehavior.totalItemQuantityAfterPickup = item.quantity + _animusAgent.inventory.GetItemQuantity(item.itemData.itemTypeId);

            // var location = AnimusEntityRegistry.Instance.GetRandom<AnimusLocation>();
            // if (location != null)
            // {
            //     moveToPosition = location.transform.position;
            //     _provider.RequestGoal<MoveGoal>();
            // }
            // else
            // {
            //     _provider.RequestGoal<WanderGoal>();
            // }
        }
    }
}