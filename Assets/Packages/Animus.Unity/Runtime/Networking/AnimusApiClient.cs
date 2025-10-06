using System.Collections.Generic;
using System.Threading.Tasks;
using Packages.Animus.Unity.Runtime.Agent.Actions;
using Packages.Animus.Unity.Runtime.Data;

namespace Packages.Animus.Unity.Runtime.Networking
{
    public class AnimusApiClient
    {
        private readonly string _baseUrl;

        public AnimusApiClient(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public Task<AgentModel> CreateAgent(AgentModel agent)
        {
            return WebRequestHandler.Post<AgentModel, AgentModel>(
                $"{_baseUrl}/agents/", agent
            );
        }

        public Task<AgentModel> GetAgentByGameKey(string gameKey)
        {
            return WebRequestHandler.Get<AgentModel>(
                $"{_baseUrl}/agents/gameKey/{gameKey}"
            );
        }

        public async Task<bool> AgentExists(string gameKey)
        {
            var agent = await GetAgentByGameKey(gameKey);
            return agent != null;
        }

        public async Task ActivateAgent(string agentId, List<string> actionKeys)
        {
            var payload = new ActivateAgentRequest { BaseActions = actionKeys };
            await WebRequestHandler.Post<ActivateAgentRequest, object>($"{_baseUrl}/agents/{agentId}/activate",
                payload);
        }
    }
}