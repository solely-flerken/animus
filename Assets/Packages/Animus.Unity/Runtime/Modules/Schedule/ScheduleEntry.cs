using System;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Modules.Schedule
{
    [Serializable]
    public class ScheduleEntry
    {
        [Tooltip("A descriptive name for the Inspector only.")]
        public string label;

        [Range(0, 23)]
        public int startHour;
        
        [Range(0, 23)]
        public int endHour;
        
        [TextArea(3, 5)] 
        [Tooltip("What the LLM will read.")]
        public string motivation;
    }
}