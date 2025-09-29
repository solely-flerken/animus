using System.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Data;
using Packages.Animus.Unity.Runtime.Networking;
using Packages.Animus.Unity.Runtime.Settings;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core
{
    public class AnimusService : IAnimusService
    {
        private readonly AnimusSettings _settings;
        private readonly AnimusApiClient _animusApi;

        public AnimusService(AnimusSettings settings)
        {
            _settings = settings;
            _animusApi = new AnimusApiClient(settings.apiServiceUrl);
        }

        public async Task<AgentModel> RegisterAgent(AgentModel agent)
        {
            var existingAgent = await _animusApi.GetAgentByGameKey(agent.gameKey);
            if (existingAgent != null)
            {
                Debug.Log("Found Agent: " + existingAgent.id);
                return existingAgent;
            }

            var createdAgent = await _animusApi.CreateAgent(agent);
            Debug.Log("Created Agent: " + createdAgent?.id);
            return createdAgent;
        }
    }
}