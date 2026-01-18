using System.Linq;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Features.Goap.Idle;
using Features.Goap.MoveTo;
using Features.Goap.Pickup;
using Features.Goap.Talk;
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
            _provider.AgentType = _goapBehaviour.GetAgentType(AgentTypes.General);

            _pickupItemBehavior = GetComponent<AgentPickupItemBehavior>();
            _talkBehaviour = GetComponent<TalkBehavior>();
            _animusAgent = GetComponent<AnimusAgent>();
        }

        private void OnEnable()
        {
            _provider.Events.OnGoalCompleted += OnGoalCompleted;
            _provider.Events.OnNoActionFound += OnNoActionFound;
        }
  
        private void OnDisable()
        {
            _provider.Events.OnGoalCompleted -= OnGoalCompleted;
            _provider.Events.OnNoActionFound -= OnNoActionFound;
        }

        private void OnGoalCompleted(IGoal goal)
        {
            _provider.RequestGoal<IdleGoal>();
        }

        private void OnNoActionFound(IGoalRequest goal)
        {
            _provider.RequestGoal<IdleGoal>();
        }
        
        private void Start()
        {
            // StartGoalTalk("Hello World!", AnimusGameManager.EntityRegistry.GetRandom<AnimusActor>());

            // StartGoalPickupItem();

            // StartGoalMoveTo(AnimusGameManager.EntityRegistry.GetAll<AnimusLocation>()[0].transform);
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

        public void StartGoalMoveTo(Transform target)
        {
            if (target != null)
            {
                moveToPosition = target;
                _provider.RequestGoal<MoveGoal>();
            }
        }
    }
}