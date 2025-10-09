using System.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Agent;
using Packages.Animus.Unity.Runtime.Agent.Actions;
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

        public async Task<AnimusEntity<T>> RegisterAgent<T>(AnimusEntity<T> agent)
        {
            var existingAgent = await _animusApi.GetAgentByGameKey<T>(agent.gameKey);
            if (existingAgent != null)
            {
                Debug.Log("Found Agent: " + existingAgent.id);
                return existingAgent;
            }

            var createdAgent = await _animusApi.CreateAgent(agent);
            Debug.Log("Created Agent: " + createdAgent?.id);
            return createdAgent;
        }

        public async Task ActivateAgent<T>(AnimusEntity<T> agent)
        {
            await _animusApi.ActivateAgent(agent.id,
                AgentRegistry.Instance.FindByGameKey(agent.gameKey).actionRegistry.GetActionKeys());
        }

        public async Task<ActionPayload> PollAction()
        {
            return await _animusApi.PollAction();
        }
    }
}