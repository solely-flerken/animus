using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Core.Utils.Json;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Event
{
    public class AnimusEvent
    {
        public AnimusEventType EventType { get; set; }

        [JsonConverter(typeof(AnimusEntityGameKeyConverter))]
        public AnimusEntity EventSource { get; set; }

        [JsonConverter(typeof(AnimusEntityListGameKeyConverter))]
        public List<AnimusEntity> EventTarget { get; set; }

        [JsonConverter(typeof(Vector3Converter))]
        public Vector3 EventLocation { get; set; }
    }
}