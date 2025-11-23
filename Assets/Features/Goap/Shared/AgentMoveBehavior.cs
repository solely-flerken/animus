using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using UnityEngine;
using UnityEngine.AI;

namespace Features.Goap.Shared
{
    [RequireComponent(typeof(NavMeshAgent), typeof(AgentBehaviour))]
    public class AgentMoveBehavior : MonoBehaviour
    {
        private const float MinMoveDistanceSqr = 0.25f * 0.25f;

        private AgentBehaviour _agentBehaviour;
        private NavMeshAgent _navMeshAgent;

        private ITarget _currentTarget;
        private Vector3 _lastPosition;

        private void Awake()
        {
            _agentBehaviour = GetComponent<AgentBehaviour>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (_currentTarget == null)
            {
                return;
            }

            var distanceToTarget = Vector3.SqrMagnitude(_currentTarget.Position - _lastPosition);
            if (distanceToTarget > MinMoveDistanceSqr)
            {
                _lastPosition = _currentTarget.Position;
                _navMeshAgent.SetDestination(_currentTarget.Position);
            }
        }

        private void OnEnable()
        {
            _agentBehaviour.Events.OnTargetLost += TargetLost;
            _agentBehaviour.Events.OnTargetChanged += OnTargetChanged;
        }

        private void OnDisable()
        {
            _agentBehaviour.Events.OnTargetLost -= TargetLost;
            _agentBehaviour.Events.OnTargetChanged -= OnTargetChanged;
        }

        private void TargetLost()
        {
            _currentTarget = null;

            if (_navMeshAgent && _navMeshAgent.isOnNavMesh)
            {
                _navMeshAgent.ResetPath();
            }
        }

        private void OnTargetChanged(ITarget target, bool inRange)
        {
            _currentTarget = target;
            _lastPosition = _currentTarget.Position;
            _navMeshAgent.SetDestination(_currentTarget.Position);
        }
    }
}