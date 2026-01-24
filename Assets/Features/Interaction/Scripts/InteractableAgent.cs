using Features.Player.Scripts;
using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Features.Interaction.Scripts
{
    [RequireComponent(typeof(AnimusAgent))]
    public class InteractableAgent : MonoBehaviour, IInteractable
    {
        private AnimusAgent Agent => GetComponent<AnimusAgent>();

        public string InteractionPrompt => $"Talk to {Agent.gameKey}";

        public void Interact(GameObject interactor)
        {
            var playerController = interactor.GetComponent<AnimusPlayerController>();
            playerController?.InitiateConversation(Agent.gameKey);
        }
    }
}