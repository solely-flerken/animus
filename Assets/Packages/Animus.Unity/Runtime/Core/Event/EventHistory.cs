using System;
using System.Collections.Generic;

namespace Packages.Animus.Unity.Runtime.Core.Event
{
    [Obsolete("Not used currently.")]
    public class EventHistory
    {
        public readonly List<AnimusEvent> Events = new();
    }
}