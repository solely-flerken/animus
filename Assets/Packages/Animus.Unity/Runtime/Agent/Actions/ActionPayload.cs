using System;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    [Serializable]
    public class ActionPayload
    {
        [JsonProperty("agent_id")] public string agentId;
        public string action;
        public string dialogue;
    }
}