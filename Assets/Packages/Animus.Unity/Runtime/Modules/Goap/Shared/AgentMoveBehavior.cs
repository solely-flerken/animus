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
        private const float MinMoveDistanceSqr = 0.25f * 0.25f;
        
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

            if (_navMeshAgent)
            {
                _navMeshAgent.ResetPath();
            }
        }

        private void OnTargetChanged(ITarget target, bool inRange)
        {
            _currentTarget = target;
            
            if (inRange)
            {
                _navMeshAgent.ResetPath();
                return;
            }
            
            _lastPosition = _currentTarget.Position;
            _navMeshAgent.SetDestination(_currentTarget.Position);
        }

        private void Update()
        {
            if (_currentTarget == null)
            {
                return;
            }

            if (Vector3.SqrMagnitude(_currentTarget.Position - _lastPosition) > MinMoveDistanceSqr)
            {
                _lastPosition = _currentTarget.Position;
                _navMeshAgent.SetDestination(_currentTarget.Position);
            }
        }
    }
}