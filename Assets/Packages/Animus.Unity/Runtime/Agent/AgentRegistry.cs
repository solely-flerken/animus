using Packages.Animus.Unity.Runtime.Core;

namespace Packages.Animus.Unity.Runtime.Agent
{
    public class AgentRegistry : TypeRegistry<AgentRegistry, Agent>
    {
        public Agent FindByGameKey(string gameKey)
        {
            return allItems.Find(a => a.animusAgent.gameKey == gameKey);
        }
    }
}