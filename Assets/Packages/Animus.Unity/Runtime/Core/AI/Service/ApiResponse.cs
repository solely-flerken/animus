using System;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Core.AI.Service
{
    [Serializable]
    public class ApiResponse
    {
        [JsonProperty("response")] public string Response { get; set; }
    }
}