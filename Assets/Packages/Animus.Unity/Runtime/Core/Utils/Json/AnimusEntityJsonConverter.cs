using System;
using System.Collections.Generic;
using System.Linq;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Core.Utils.Json
{
    public class AnimusEntityGameKeyConverter : JsonConverter<AnimusEntity>
    {
        public override void WriteJson(JsonWriter writer, AnimusEntity value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(value.gameKey);
            }
        }

        public override AnimusEntity ReadJson(JsonReader reader, Type objectType, AnimusEntity existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class AnimusEntityListGameKeyConverter<T> : JsonConverter<List<T>> where T : AnimusEntity
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