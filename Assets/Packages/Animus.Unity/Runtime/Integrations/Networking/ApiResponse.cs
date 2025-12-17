using System;
using Newtonsoft.Json;
using Packages.Animus.Unity.Runtime.Core.Actions;

namespace Packages.Animus.Unity.Runtime.Integrations.Networking
{
    [Serializable]
    public class ApiResponse
    {
        [JsonProperty("payload")]
        public ActionPayload Payload { get; set; }
    }
}