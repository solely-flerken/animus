using System;
using System.Collections.Generic;
using System.Linq;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Core.Utils.Json
{
    public class AnimusEntityListDetailsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            // This converter can handle any List where the element type derives from AnimusEntity.
            if (!objectType.IsGenericType || objectType.GetGenericTypeDefinition() != typeof(List<>))
            {
                return false;
            }

            var itemType = objectType.GetGenericArguments()[0];
            return typeof(AnimusEntity).IsAssignableFrom(itemType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var list = (IEnumerable<AnimusEntity>)value;
            var itemType = value.GetType().GetGenericArguments()[0];

            writer.WriteStartObject();

            // Use the class name of the list's items as the "type".
            writer.WritePropertyName("type");
            writer.WriteValue(itemType.Name);

            // Serialize the list of entities into the "items" array.
            writer.WritePropertyName("items");
            var objectsToSerialize = list.Select(e => new { e.gameKey, e.description }).ToList();
            serializer.Serialize(writer, objectsToSerialize);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}