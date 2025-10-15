using System;
using Packages.Animus.Unity.Runtime.Agent.Actions;
using Packages.Animus.Unity.Runtime.Core;
using Packages.Animus.Unity.Runtime.Environment.PointOfInterest;
using UnityEngine;
using UnityEngine.AI;

namespace Packages.Animus.Unity.Runtime.Agent
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Agent : MonoBehaviour
    {
        public AnimusEntity<AgentDetails> agentEntity;

        public ActionRegistry actionRegistry;

        private NavMeshAgent _navMeshAgent;
        private Vector3 _currentTargetPosition;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            AgentRegistry.Instance.Register(this);
            actionRegistry.Initialize();

            RegisterAsync();
        }

        private async void RegisterAsync()
        {
            try
            {
                var service = AnimusServiceManager.Service;
                agentEntity = await service.RegisterAgent(agentEntity);

                await service.ActivateAgent(agentEntity);
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