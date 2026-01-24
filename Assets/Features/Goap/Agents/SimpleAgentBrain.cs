using System;
using System.Linq;
using System.Threading;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using Cysharp.Threading.Tasks;
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

        private UniTaskCompletionSource<bool> _activeGoalSource;
        
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
            _activeGoalSource?.TrySetResult(true); 
            _activeGoalSource = null;
            
            _provider.RequestGoal<IdleGoal>();
        }

        private void OnNoActionFound(IGoalRequest goal)
        {
            _activeGoalSource?.TrySetResult(false); 
            _activeGoalSource = null;
            
            _provider.RequestGoal<IdleGoal>();
        }
        
        private void Start()
        {
            // StartGoalTalk("Hello World!", AnimusGameManager.EntityRegistry.GetRandom<AnimusActor>());

            // StartGoalPickupItem();

            // StartGoalMoveTo(AnimusGameManager.EntityRegistry.GetAll<AnimusLocation>()[0].transform);
        }

        public UniTask StartGoalIdle(CancellationToken token)
        {
            CancelActiveGoalSource();
            
            _provider.RequestGoal<IdleGoal>();

            return WaitWithCancellation(token);
        }
        
        public UniTask StartGoalTalk(string text, AnimusActor targetActor, CancellationToken token)
        {
            CancelActiveGoalSource();
            _activeGoalSource = new UniTaskCompletionSource<bool>();
            
            _talkBehaviour.text = text;
            _talkBehaviour.targetActor = targetActor;
            _talkBehaviour.hasFinishedTalking = false;
            moveToPosition = targetActor.transform;
            
            _provider.RequestGoal<TalkGoal>();
            
            return WaitWithCancellation(token);
        }

        public UniTask StartGoalPickupItem(AnimusObject item, CancellationToken token)
        {
            CancelActiveGoalSource();
            _activeGoalSource = new UniTaskCompletionSource<bool>();
            
            _pickupItemBehavior.targetItemTypeId = item.itemData.itemTypeId;
            _pickupItemBehavior.targetItem = item;
            _pickupItemBehavior.totalItemQuantityAfterPickup = item.quantity + _animusAgent.inventory.GetItemQuantity(item.itemData.itemTypeId);
            
            _provider.RequestGoal<PickupItemGoal>();
            
            return WaitWithCancellation(token);
        }

        public UniTask StartGoalMoveTo(Transform target, CancellationToken token)
        {
            CancelActiveGoalSource();
            _activeGoalSource = new UniTaskCompletionSource<bool>();
            
            moveToPosition = target;
            
            _provider.RequestGoal<MoveGoal>();
            
            return WaitWithCancellation(token);
        }
        
        private void CancelActiveGoalSource()
        {
            _activeGoalSource?.TrySetCanceled();
            _activeGoalSource = null;
        }

        private async UniTask WaitWithCancellation(CancellationToken token)
        {
            try
            {
                await _activeGoalSource.Task.AttachExternalCancellation(token);
            }
            catch (OperationCanceledException)
            {
                _provider.RequestGoal<IdleGoal>();
                throw;
            }
        }
    }
}