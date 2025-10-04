using System;
using Packages.Animus.Unity.Runtime.Core;
using Packages.Animus.Unity.Runtime.Data;
using Packages.Animus.Unity.Runtime.Environment.PointOfInterest;
using UnityEngine;
using UnityEngine.AI;

namespace Packages.Animus.Unity.Runtime.Agent
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Agent : MonoBehaviour
    {
        public AgentModel agentModel;

        public InstructionRegistry instructionRegistry;

        private NavMeshAgent _navMeshAgent;
        private Vector3 _currentTargetPosition;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            AgentRegistry.Instance.Register(this);
            instructionRegistry.Initialize();

            RegisterAsync();
        }

        private async void RegisterAsync()
        {
            try
            {
                var service = AnimusServiceManager.Service;
                agentModel = await service.RegisterAgent(agentModel);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void OnDisable()
        {
            AgentRegistry.Instance?.Unregister(this);
        }

        public void GoToPoi(PointOfInterest poi)
        {
            _currentTargetPosition = poi.transform.position;
            _navMeshAgent.SetDestination(_currentTargetPosition);
        }
    }
}