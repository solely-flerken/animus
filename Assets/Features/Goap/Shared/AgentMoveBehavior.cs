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

        private Animator _animator;
        private float NormalizedSpeed => _navMeshAgent.velocity.magnitude / _navMeshAgent.speed;
        private static readonly int Forward = Animator.StringToHash("Forward");
        
        private ITarget _currentTarget;
        private Vector3 _lastPosition;
        private bool _isInRange;
        
        private void Awake()
        {
            _agentBehaviour = GetComponent<AgentBehaviour>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (_currentTarget != null && !_isInRange)
            {
                var distanceToTarget = Vector3.SqrMagnitude(_currentTarget.Position - _lastPosition);
                if (distanceToTarget > MinMoveDistanceSqr)
                {
                    _lastPosition = _currentTarget.Position;
                    _navMeshAgent.SetDestination(_currentTarget.Position);
                }
            }
            else if (_isInRange && _navMeshAgent.hasPath)
            {
                _navMeshAgent.ResetPath();
            }
            
            _animator.SetFloat(Forward, NormalizedSpeed);
        }

        private void OnEnable()
        {
            _agentBehaviour.Events.OnTargetLost += TargetLost;
            _agentBehaviour.Events.OnTargetChanged += OnTargetChanged;
            _agentBehaviour.Events.OnTargetInRange += OnTargetInRange;
            _agentBehaviour.Events.OnTargetNotInRange += OnTargetNotInRange; 
        }

        private void OnDisable()
        {
            _agentBehaviour.Events.OnTargetLost -= TargetLost;
            _agentBehaviour.Events.OnTargetChanged -= OnTargetChanged;
            _agentBehaviour.Events.OnTargetInRange -= OnTargetInRange;
            _agentBehaviour.Events.OnTargetNotInRange -= OnTargetNotInRange; 
        }

        private void TargetLost()
        {
            _currentTarget = null;
            _isInRange = false;

            if (_navMeshAgent && _navMeshAgent.isOnNavMesh)
            {
                _navMeshAgent.ResetPath();
            }
        }

        private void OnTargetChanged(ITarget target, bool inRange)
        {
            _currentTarget = target;
            _isInRange = inRange;
            _lastPosition = _currentTarget.Position;
            
            if (!inRange)
            {
                _navMeshAgent.SetDestination(_currentTarget.Position);
            }
            else
            {
                _navMeshAgent.ResetPath();
            }
        }
        
        private void OnTargetInRange(ITarget target)
        {
            _isInRange = true;
            if (_navMeshAgent && _navMeshAgent.isOnNavMesh)
            {
                _navMeshAgent.ResetPath();
            }
        }
        
        private void OnTargetNotInRange(ITarget target)
        {
            _isInRange = false;
        }
    }
}