using UnityEngine.UIElements;

namespace Core.UI.Scripts
{
    public interface IUserInterface
    {
        VisualElement Root { get; set; }
        bool IsVisible => Root.style.display == DisplayStyle.Flex;
    }
}