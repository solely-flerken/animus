using System.Collections.Generic;

namespace Packages.Animus.Unity.Runtime.Core.Event
{
    public class AnimusEvent
    {
        public AnimusEventType EventType { get; set; }
        public AnimusEntity EventSource { get; set; }
        public List<AnimusEntity> EventTarget { get; set; }
    }
}