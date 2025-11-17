using UnityEngine;
using UnityEngine.UIElements;

namespace Core.UI.Scripts
{
    public abstract class UserInterfaceBase : MonoBehaviour, IUserInterface
    {
        public VisualElement Root { get; set; }
        public bool IsVisibleInitially { get; set; }
        public bool IsVisible => Root?.style.display == DisplayStyle.Flex;

        public virtual void Show()
        {
            Root.style.display = DisplayStyle.Flex;
        }

        public virtual void Hide()
        {
            Root.style.display = DisplayStyle.None;
        }
    }
}