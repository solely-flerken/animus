using System;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Core.Utils.TypeSelection
{
    public class SerializableTypeConverter : JsonConverter<SerializableType>
    {
        public override void WriteJson(JsonWriter writer, SerializableType value, JsonSerializer serializer)
        {
            if (value?.Type == null)
            {
                writer.WriteNull();
                return;
            }
            
            writer.WriteValue(value.Type.Name);
        }

        public override SerializableType ReadJson(JsonReader reader, Type objectType, SerializableType existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}