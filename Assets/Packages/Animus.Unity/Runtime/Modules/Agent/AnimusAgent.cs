using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Core.Actions;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Core.Event;
using Packages.Animus.Unity.Runtime.Core.Memory;
using Packages.Animus.Unity.Runtime.Modules.Environment;
using UnityEngine;
using UnityEngine.AI;

namespace Packages.Animus.Unity.Runtime.Modules.Agent
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AnimusAgent : AnimusActor
    {
        public override AnimusEntityType Type => AnimusEntityType.Agent;

        [TextArea(3, 10)] public string persona;
        
        [Header("NPC Perception")]
        public float perceptionRadius = 20f;
        public float fieldOfViewAngle = 120f;
        public LayerMask obstacleLayer;

        public static ConversationHistory SharedHistory;
        public ActionHistory actionHistory;
        public EventHistory eventHistory;
        public List<string> memories;

        public AgentActionRunner actionRunner;
        
        private NavMeshAgent _navMeshAgent;
        private Vector3 _currentTargetPosition;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            actionRunner = GetComponent<AgentActionRunner>();
        }

        private void Start()
        {
            AnimusEntityRegistry.Instance.Register(this);

            SharedHistory = new ConversationHistory(50);
            actionHistory = new ActionHistory();
            eventHistory = new EventHistory();
        }

        private void OnDisable()
        {
            AnimusEntityRegistry.Instance?.Unregister(this);
        }

        public async UniTask<bool> GoToPoi(AnimusLocation poi, CancellationToken cancellationToken = default)
        {
            if (poi == null) return false;
            
            _currentTargetPosition = poi.transform.position;
            _navMeshAgent.SetDestination(_currentTargetPosition);
            
            while (_navMeshAgent.pathPending)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
            
            while (_navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance)
            {
                if (_navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete)
                {
                    Debug.LogError("Navigation path failed.");
                    return false;
                }
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
            
            return _navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete;
        }
    }
}