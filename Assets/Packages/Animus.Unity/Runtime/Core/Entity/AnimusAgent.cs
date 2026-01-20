using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Core.Actions;
using Packages.Animus.Unity.Runtime.Core.Config.Script;
using Packages.Animus.Unity.Runtime.Modules.Conversation;
using Packages.Animus.Unity.Runtime.Modules.Schedule;
using UnityEngine;
using UnityEngine.AI;

namespace Packages.Animus.Unity.Runtime.Core.Entity
{
    [RequireComponent(typeof(NavMeshAgent), typeof(AgentActionSystem))]
    public class AnimusAgent : AnimusActor
    {
        public override AnimusEntityType Type => AnimusEntityType.Agent;

        [TextArea(3, 10)] public string persona;
        
        [Header("NPC Perception")]
        public float perceptionRadius = 20f;
        public float fieldOfViewAngle = 120f;
        public LayerMask obstacleLayer;

        public NpcSchedule npcSchedule;
        public string currentMotivation = "You have no special motivation.";
        public string currentActionResult = "None.";
        public List<string> memories;
        public static ConversationHistory SharedHistory;

        public AgentActionSystem agentActionSystem;
        private NavMeshAgent _navMeshAgent;
        
        private Vector3 _currentTargetPosition;

        private void Awake()
        {
            if (TryGetComponent<NpcSchedule>(out var schedule))
            {
                npcSchedule = schedule;
            }
            
            agentActionSystem = GetComponent<AgentActionSystem>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            SharedHistory = new ConversationHistory(50);
        }

        private void OnEnable()
        {
            AnimusGameManager.EntityRegistry?.Register(this);
        }

        private void OnDisable()
        {
            AnimusGameManager.EntityRegistry?.Unregister(this);
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