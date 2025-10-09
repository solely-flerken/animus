using System;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core
{
    [Serializable]
    public class AnimusEntity<T>
    {
        public string name;
        [JsonProperty("game_key")] public string gameKey;
        [TextArea(3, 10)] public string description;
        public string type;
        public T details;

        [HideInInspector] public string id;
        [HideInInspector] public string createdAt;
        [HideInInspector] public string updatedAt;
    }
}