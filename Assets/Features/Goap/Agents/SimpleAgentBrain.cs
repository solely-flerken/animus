using System.Linq;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Features.Goap.MoveTo;
using Features.Goap.Pickup;
using Features.Goap.Talk;
using Features.Goap.Wander;
using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Features.Goap.Agents
{
    public class SimpleAgentBrain : MonoBehaviour
    {
        private GoapBehaviour _goapBehaviour;
        private GoapActionProvider _provider;

        private AgentPickupItemBehavior _pickupItemBehavior;
        private TalkBehavior _talkBehaviour;
        private AnimusAgent _animusAgent;

        public Transform moveToPosition;

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
            // StartGoalTalk("Hello World!", AnimusGameManager.EntityRegistry.GetRandom<AnimusActor>());

            // StartGoalPickupItem();

            // StartGoalMoveTo();
        }

        public void StartGoalTalk(string text, AnimusActor targetActor)
        {
            _talkBehaviour.text = text;
            _talkBehaviour.targetActor = targetActor;
            _talkBehaviour.hasFinishedTalking = false;
            moveToPosition = targetActor.transform;
            _provider.RequestGoal<TalkGoal>();
        }

        public void StartGoalPickupItem(AnimusObject item)
        {
            _pickupItemBehavior.targetItemTypeId = item.itemData.itemTypeId;
            _pickupItemBehavior.targetItem = item;
            _pickupItemBehavior.totalItemQuantityAfterPickup = item.quantity + _animusAgent.inventory.GetItemQuantity(item.itemData.itemTypeId);
            _provider.RequestGoal<PickupItemGoal>();
        }

        public void StartGoalMoveTo(AnimusEntity entity)
        {
            if (entity != null)
            {
                moveToPosition = entity.transform;
                _provider.RequestGoal<MoveGoal>();
            }
            else
            {
                _provider.RequestGoal<WanderGoal>();
            }
        }
    }
}