using UnityEngine.UIElements;

namespace UI
{
    public interface IUserInterface
    {
        VisualElement Root { get; set; }
        bool IsVisible => Root.style.display == DisplayStyle.Flex;
    }
}