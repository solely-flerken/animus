using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Event
{
    public class AnimusEvent
    {
        public AnimusEventType EventType { get; set; }
        public AnimusEntity EventSource { get; set; }
        public List<AnimusEntity> EventTarget { get; set; }
        public Vector3 EventLocation { get; set; }
    }
}