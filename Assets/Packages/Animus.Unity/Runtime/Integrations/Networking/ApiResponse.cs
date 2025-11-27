using System;
using Packages.Animus.Unity.Runtime.Core.Actions;
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