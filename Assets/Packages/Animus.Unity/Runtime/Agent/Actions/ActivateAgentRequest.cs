using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    public class ActivateAgentRequest
    {
        [JsonProperty("base_actions")] public List<string> BaseActions { get; set; }
    }
}