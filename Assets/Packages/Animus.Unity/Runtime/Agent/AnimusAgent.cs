using Packages.Animus.Unity.Runtime.Agent.Actions;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Core.Event;
using Packages.Animus.Unity.Runtime.Core.Memory;
using Packages.Animus.Unity.Runtime.Environment;
using UnityEngine;
using UnityEngine.AI;

namespace Packages.Animus.Unity.Runtime.Agent
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AnimusAgent : AnimusEntity
    {
        public override AnimusEntityType Type => AnimusEntityType.Agent;

        [TextArea(3, 10)] public string persona;

        public ActionCollection actionCollection;
        
        [Header("NPC Perception")]
        public float perceptionRadius = 20f;
        public float fieldOfViewAngle = 120f;
        public LayerMask obstacleLayer;

        public ConversationHistory conversationHistory;
        public EventHistory eventHistory;

        private NavMeshAgent _navMeshAgent;
        private Vector3 _currentTargetPosition;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            AnimusEntityRegistry.Instance.Register(this);

            conversationHistory = new ConversationHistory(50);
            eventHistory = new EventHistory();
            actionCollection.Initialize();
        }

        private void OnDisable()
        {
            AnimusEntityRegistry.Instance?.Unregister(this);
        }

        public void GoToPoi(AnimusLocation poi)
        {
            _currentTargetPosition = poi.transform.position;
            _navMeshAgent.SetDestination(_currentTargetPosition);
        }
    }
}