using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Infrastructure.Serialization
{
    public static class JsonUtility
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = new IgnoreUnityBasePropertiesResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };
        
        /// <summary>
        /// Serializes an object to a JSON string using the settings.
        /// </summary>
        public static string Serialize<T>(T obj, bool prettyPrint = false)
        {
            var formatting = prettyPrint ? Formatting.Indented : Formatting.None;
            return JsonConvert.SerializeObject(obj, formatting, Settings);
        }

        /// <summary>
        /// Deserializes a JSON string to an object of type T using the settings.
        /// </summary>
        public static T Deserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            try
            {
                // Our consistent settings are applied here automatically.
                return JsonConvert.DeserializeObject<T>(json, Settings);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to deserialize JSON to type {typeof(T).Name}. Error: {e.Message}\nJSON: {json}");
                return default;
            }
        }
    }
}