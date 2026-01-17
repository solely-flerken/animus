using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Modules.GameTime;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Modules.Schedule
{
    public class NpcSchedule : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private List<ScheduleEntry> dailyRoutine = new();
        [SerializeField] private string @default = "You have no specific obligations right now.";

        public ScheduleEntry GetCurrentEntry()
        {
            var currentHour = TimeManager.Instance.CurrentHour;
            return dailyRoutine.Find(e => currentHour >= e.startHour && currentHour < e.endHour);
        }

        public string GetScheduleContext()
        {
            var currentHour = TimeManager.Instance.CurrentHour;

            var entry = dailyRoutine.Find(e => currentHour >= e.startHour && currentHour < e.endHour);

            var motivation = entry != null ? entry.motivation : @default;

            return motivation;
        }
    }
}