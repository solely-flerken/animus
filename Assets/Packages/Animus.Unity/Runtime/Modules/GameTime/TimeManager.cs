using System;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Modules.GameTime
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        [Header("Time Settings")]
        [SerializeField] private float dayDurationInMinutes = 10f; // Real-time minutes for one full day
        [SerializeField] private float startHour = 6f; // Start at 6 AM
    
        [Header("Current Time")]
        [SerializeField] private float currentTime; // Time in hours (0-24)
        [SerializeField] private int currentDay = 1;
        
        private float _timeMultiplier;
        private int _previousHour;
        private int _previousMinute;
        
        public event Action OnMinuteChanged; 
        public event Action<int> OnHourChanged;
        public event Action<int> OnDayChanged;
        public event Action OnDayStarted;
        public event Action OnNightStarted;
        public event Action OnDayEnded;
        public event Action OnNightEnded;
        
        // Properties
        public float CurrentTime => currentTime;
        public int CurrentDay => currentDay;
        public int CurrentHour => Mathf.FloorToInt(currentTime);
        public int CurrentMinute => Mathf.FloorToInt((currentTime % 1) * 60f);
        public bool IsDay => currentTime is >= 6f and < 18f;
        public bool IsNight => !IsDay;
        public float DayProgress => currentTime / 24f; // 0 to 1
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        
            Initialize();
        }

        private void Initialize()
        {
            currentTime = startHour;
            _previousHour = CurrentHour;
            _previousMinute = CurrentMinute;
        
            // Calculate how fast time should progress
            // 24 in-game hours should pass in 'dayDurationInMinutes' real-time minutes
            _timeMultiplier = 24f / (dayDurationInMinutes * 60f);
        }

        private void Update()
        {
            UpdateTime();
            CheckHourChange();
        }

        private void UpdateTime()
        {
            currentTime += Time.deltaTime * _timeMultiplier;
        
            // Wrap around after 24 hours
            if (currentTime >= 24f)
            {
                currentTime -= 24f;
                currentDay++;
                OnDayChanged?.Invoke(currentDay);
            }
        }

        private void CheckHourChange()
        {
            var currentHourInt = CurrentHour;
            var currentMinuteInt = CurrentMinute;
            
            if (currentHourInt != _previousHour)
            {
                OnHourChanged?.Invoke(currentHourInt);
            
                // Check for day/night transitions
                if (currentHourInt == 6)
                {
                    OnNightEnded?.Invoke();
                    OnDayStarted?.Invoke();
                }
                else if (currentHourInt == 18)
                {
                    OnDayEnded?.Invoke();
                    OnNightStarted?.Invoke();
                }
            
                _previousHour = currentHourInt;
            }
            
            if (currentMinuteInt != _previousMinute)
            {
                OnMinuteChanged?.Invoke();
                _previousMinute = currentMinuteInt;
            }
        }

        public void SetTime(float hour)
        {
            currentTime = Mathf.Clamp(hour, 0f, 24f);
        }

        public void SetDayDuration(float minutes)
        {
            dayDurationInMinutes = minutes;
            _timeMultiplier = 24f / (dayDurationInMinutes * 60f);
        }

        public string GetFormattedTime()
        {
            return $"{CurrentHour:00}:{CurrentMinute:00}";
        }

        public float GetNormalizedTime()
        {
            return currentTime / 24f;
        }
    }
}