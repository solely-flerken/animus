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

    public class AnimusEntityListGameKeyConverter : JsonConverter<List<AnimusEntity>>
    {
        public override void WriteJson(JsonWriter writer, List<AnimusEntity> value, JsonSerializer serializer)
        {
            var keys = value?.Select(e => e?.gameKey).ToList() ?? new List<string>();
            serializer.Serialize(writer, keys);
        }

        public override List<AnimusEntity> ReadJson(JsonReader reader, Type objectType,
            List<AnimusEntity> existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}