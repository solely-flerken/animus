using System;
using System.Collections.Generic;
using System.Linq;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Core.Utils.Json
{
    public class AnimusEntityGameKeyListConverter<T> : JsonConverter<List<T>> where T : AnimusEntity
    {
        public override void WriteJson(JsonWriter writer, List<T> value, JsonSerializer serializer)
        {
            var keys = value?.Select(e => e?.gameKey).ToList() ?? new List<string>();
            serializer.Serialize(writer, keys);
        }

        public override List<T> ReadJson(JsonReader reader, Type objectType, List<T> existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}