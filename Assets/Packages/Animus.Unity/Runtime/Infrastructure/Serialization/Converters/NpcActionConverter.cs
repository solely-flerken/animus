using System;
using Packages.Animus.Unity.Runtime.Modules.Agent.Actions;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Infrastructure.Serialization.Converters
{
    public class NpcActionConverter : JsonConverter<NpcAction>
    {
        public override void WriteJson(JsonWriter writer, NpcAction value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();

            writer.WritePropertyName("actionKey");
            writer.WriteValue(value.actionKey);

            writer.WritePropertyName("description");
            writer.WriteValue(value.description);

            writer.WritePropertyName("parameters");
            serializer.Serialize(writer, value.parameters);

            writer.WriteEndObject();
        }

        public override NpcAction ReadJson(JsonReader reader, Type objectType, NpcAction existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}