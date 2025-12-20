using Core.UI.Scripts;
using UnityEngine.UIElements;

namespace Features.GameTime.Scripts
{
    public class TimeUI : UserInterfaceBase
    {
        private Label _timeLabel;

        private void Start()
        {
            Root = GetComponent<UIDocument>().rootVisualElement;
            _timeLabel = Root.Q<Label>("clock");

            if (TimeManager.Instance == null)
            {
                Hide();
            }
            else
            {
                var timeProperty = BindableProperty<string>.Bind(TimeManager.Instance.GetFormattedTime);
                _timeLabel.Bind(timeProperty, nameof(Label.text));
                Show();
            }
        }
    }
}