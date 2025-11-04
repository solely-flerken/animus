using System;
using Packages.Animus.Unity.Runtime.Agent.Actions;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Core.AI.Service
{
    [Serializable]
    public class ApiResponse
    {
        [JsonProperty("payload")] public ActionPayload<string> Payload { get; set; }
    }
}