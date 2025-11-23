using System.Linq;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Modules.Agent;
using Packages.Animus.Unity.Runtime.Modules.Environment;
using Packages.Animus.Unity.Runtime.Modules.Goap.MoveTo;
using Packages.Animus.Unity.Runtime.Modules.Goap.Pickup;
using Packages.Animus.Unity.Runtime.Modules.Goap.Talk;
using Packages.Animus.Unity.Runtime.Modules.Goap.Wander;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Modules.Goap.Agents
{
    public class SimpleAgentBrain : MonoBehaviour
    {
        private GoapBehaviour _goapBehaviour;
        private GoapActionProvider _provider;

        private AgentPickupItemBehavior _pickupItemBehavior;
        private TalkBehavior _talkBehaviour;
        private AnimusAgent _animusAgent;

        public Vector3? moveToPosition;

        private void Awake()
        {
            _goapBehaviour = FindObjectsByType<GoapBehaviour>(FindObjectsSortMode.None).First();

            _provider = GetComponent<GoapActionProvider>();
            _provider.AgentType = _goapBehaviour.GetAgentType(AgentConstants.General);

            _pickupItemBehavior = GetComponent<AgentPickupItemBehavior>();
            _talkBehaviour = GetComponent<TalkBehavior>();
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
            StartGoalTalk();

            // StartGoalPickupItem();

            // StartGoalMoveTo();
        }

        public void StartGoalTalk()
        {
            _talkBehaviour.text = "Hello World!";
            _talkBehaviour.targetActor = AnimusEntityRegistry.Instance.GetRandom<AnimusActor>();
            _talkBehaviour.hasFinishedTalking = false;
            _provider.RequestGoal<TalkGoal>();
        }

        public void StartGoalPickupItem()
        {
            var item = AnimusEntityRegistry.Instance.GetRandom<AnimusObject>();
            _pickupItemBehavior.targetItemTypeId = item.itemData.itemTypeId;
            _pickupItemBehavior.targetItem = item;
            _pickupItemBehavior.totalItemQuantityAfterPickup = item.quantity + _animusAgent.inventory.GetItemQuantity(item.itemData.itemTypeId);
            _provider.RequestGoal<PickupItemGoal>();
        }

        public void StartGoalMoveTo()
        {
            var location = AnimusEntityRegistry.Instance.GetRandom<AnimusLocation>();
            if (location != null)
            {
                moveToPosition = location.transform.position;
                _provider.RequestGoal<MoveGoal>();
            }
            else
            {
                _provider.RequestGoal<WanderGoal>();
            }
        }
    }
}