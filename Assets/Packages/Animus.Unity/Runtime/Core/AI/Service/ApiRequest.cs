using System;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Core.AI.Service
{
    [Serializable]
    public class ApiRequest<T>
    {
        [JsonProperty("payload")] public T Payload { get; set; }
    }
}