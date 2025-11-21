using System;
using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Agent.Actions;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Integrations.Networking
{
    [Serializable]
    public class ModelResponse
    {
        [JsonProperty("actions")]
        public List<ActionPayload> Actions { get; set; }
    }
}