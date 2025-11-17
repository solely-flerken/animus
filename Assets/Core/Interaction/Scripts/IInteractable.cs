using UnityEngine;

namespace Core.Interaction.Scripts
{
    public interface IInteractable
    {
        string InteractionPrompt { get; }
        void Interact(GameObject interactor);
    }
}