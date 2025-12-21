using Core.UI.Scripts;
using Packages.Animus.Unity.Runtime.Modules.GameTime;
using UnityEngine.UIElements;

namespace Features.GameTime.Scripts
{
    public class TimeUI : UserInterfaceBase
    {
        private static TimeManager TimeManagerInstance => TimeManager.Instance;

        private Label _timeLabel;

        private void Start()
        {
            Root = GetComponent<UIDocument>().rootVisualElement;
            _timeLabel = Root.Q<Label>("clock");

            if (TimeManagerInstance == null)
            {
                Hide();
                return;
            }

            UpdateTimeDisplay();
            TimeManagerInstance.OnMinuteChanged += UpdateTimeDisplay;
        }

        private void OnDestroy()
        {
            if (TimeManagerInstance != null)
            {
                TimeManagerInstance.OnMinuteChanged -= UpdateTimeDisplay;
            }
        }

        private void UpdateTimeDisplay()
        {
            _timeLabel.text = TimeManagerInstance.GetFormattedTime();
        }
    }
}