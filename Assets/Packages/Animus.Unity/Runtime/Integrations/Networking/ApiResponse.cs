using System;
using Packages.Animus.Unity.Runtime.Modules.Agent.Actions;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Integrations.Networking
{
    [Serializable]
    public class ApiResponse
    {
        [JsonProperty("payload")]
        public ActionPayload Payload { get; set; }
    }
}