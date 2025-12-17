using System.Collections.Generic;
using Newtonsoft.Json;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Infrastructure.Serialization.Converters;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Event
{
    public class AnimusEvent
    {
        public AnimusEventType EventType { get; set; }

        [JsonConverter(typeof(AnimusEntityGameKeyConverter))]
        public AnimusEntity EventSource { get; set; }

        [JsonConverter(typeof(AnimusEntityGameKeyListConverter<AnimusEntity>))]
        public List<AnimusEntity> EventTarget { get; set; }

        [JsonConverter(typeof(Vector3Converter))]
        public Vector3 EventLocation { get; set; }
    }
}