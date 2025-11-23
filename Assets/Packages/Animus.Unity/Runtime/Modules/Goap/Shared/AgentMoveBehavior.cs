using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using UnityEngine;
using UnityEngine.AI;

namespace Packages.Animus.Unity.Runtime.Modules.Goap.Shared
{
    [RequireComponent(typeof(NavMeshAgent), typeof(AgentBehaviour))]
    public class AgentMoveBehavior : MonoBehaviour
    {
        private AgentBehaviour _agentBehaviour;
        private ITarget _currentTarget;

        private NavMeshAgent _navMeshAgent;
        private Vector3 _lastPosition;
        private const float MinMoveDistance = 0.25f;
        
        private void Awake()
        {
            _agentBehaviour = GetComponent<AgentBehaviour>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
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
        }

        private void OnTargetChanged(ITarget target, bool inRange)
        {
            _currentTarget = target;
            _lastPosition = _currentTarget.Position;
            _navMeshAgent.SetDestination(target.Position);
        }

        private void Update()
        {
            if (_currentTarget == null)
            {
                return;
            }

            if (MinMoveDistance <= Vector3.Distance(_currentTarget.Position, _lastPosition))
            {
                _lastPosition = _currentTarget.Position;
                _navMeshAgent.SetDestination(_currentTarget.Position);
            }
        }
    }
}