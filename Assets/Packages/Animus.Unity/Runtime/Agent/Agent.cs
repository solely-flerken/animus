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

        private NavMeshAgent _navMeshAgent;
        private Vector3 _currentTargetPosition;

        private async void Start()
        {
            try
            {
                _navMeshAgent = GetComponent<NavMeshAgent>();

                var service = AnimusServiceManager.Service;
                agentModel = await service.RegisterAgent(agentModel);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void GoToPoi(PointOfInterest poi)
        {
            _currentTargetPosition = poi.transform.position;
            _navMeshAgent.SetDestination(_currentTargetPosition);
        }
    }
}