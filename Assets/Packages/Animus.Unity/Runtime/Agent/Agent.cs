using System;
using Packages.Animus.Unity.Runtime.Core;
using Packages.Animus.Unity.Runtime.Data;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent
{
    public class Agent : MonoBehaviour
    {
        public AgentModel agentModel;

        private async void Start()
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
    }
}