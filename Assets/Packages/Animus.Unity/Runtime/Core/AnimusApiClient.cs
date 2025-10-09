using System.Collections.Generic;
using System.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Agent.Actions;
using Packages.Animus.Unity.Runtime.Networking;

namespace Packages.Animus.Unity.Runtime.Core
{
    public class AnimusApiClient
    {
        private readonly string _baseUrl;

        public AnimusApiClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public Task<AnimusEntity<T>> CreateAgent<T>(AnimusEntity<T> agent)
        {
            return WebRequestHandler.Post<AnimusEntity<T>, AnimusEntity<T>>(
                $"{_baseUrl}/agents/", agent
            );
        }

        public Task<AnimusEntity<T>> GetAgentByGameKey<T>(string gameKey)
        {
            return WebRequestHandler.Get<AnimusEntity<T>>(
                $"{_baseUrl}/agents/gameKey/{gameKey}"
            );
        }

        public async Task<bool> AgentExists<T>(string gameKey)
        {
            var agent = await GetAgentByGameKey<T>(gameKey);
            return agent != null;
        }

        public async Task ActivateAgent(string agentId, List<string> actionKeys)
        {
            var payload = new ActivateAgentRequest { BaseActions = actionKeys };
            await WebRequestHandler.Post<ActivateAgentRequest, object>($"{_baseUrl}/agents/{agentId}/activate",
                payload);
        }
        
        public async Task<ActionPayload> PollAction()
        {
            return await WebRequestHandler.Get<ActionPayload>($"{_baseUrl}/actions");
        }
    }
}