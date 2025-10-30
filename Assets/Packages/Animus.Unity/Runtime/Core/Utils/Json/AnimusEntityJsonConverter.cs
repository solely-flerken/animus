using System;
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
}