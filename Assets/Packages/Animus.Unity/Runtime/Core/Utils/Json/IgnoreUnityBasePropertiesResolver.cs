using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Serialization;

namespace Packages.Animus.Unity.Runtime.Core.Utils.Json
{
    public class IgnoreUnityBasePropertiesResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type,
            MemberSerialization memberSerialization)
        {
            // Get the properties that Newtonsoft would normally serialize
            var properties = base.CreateProperties(type, memberSerialization);

            // If the type isn't a UnityEngine.Object, we don't need to do anything.
            if (!typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                return properties;
            }

            // For Unity Objects, filter out properties that are declared on the base
            // Unity types we want to ignore.
            var filteredProperties = properties.Where(p =>
                p.DeclaringType != typeof(UnityEngine.Object) &&
                p.DeclaringType != typeof(UnityEngine.ScriptableObject)
            ).ToList();

            return filteredProperties;
        }
    }
}