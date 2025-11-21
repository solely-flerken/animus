using UnityEngine;

namespace Features.Interaction.Scripts
{
    public interface IInteractable
    {
        string InteractionPrompt { get; }
        void Interact(GameObject interactor);
    }
}