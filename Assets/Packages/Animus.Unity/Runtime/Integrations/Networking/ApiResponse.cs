using System;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Integrations.Networking
{
    [Serializable]
    public class ApiResponse
    {
        [JsonProperty("payload")]
        public ModelResponse Payload { get; set; }
    }
}