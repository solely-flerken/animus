using Core.UI.Scripts;
using UnityEngine.UIElements;

namespace Core.Interaction.Scripts
{
    public class InteractionUIManager : UserInterfaceBase
    {
        public static InteractionUIManager Instance;
        
        private VisualElement _container;
        private Label _interactionPromptLabel;

        private Interactable _currentInteractable;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            Root = GetComponent<UIDocument>().rootVisualElement;
            _container = Root.Q<VisualElement>("container");
            _interactionPromptLabel = Root.Q<Label>("interactionPromptLabel");
        }

        public void Show(string message)
        {
            _interactionPromptLabel.text = message;
            Show();
        }
    }
}