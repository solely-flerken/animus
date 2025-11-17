using Features.Chat.Scripts;
using Packages.Animus.Unity.Runtime.Agent;
using UnityEngine;

namespace Core.Interaction.Scripts
{
    [RequireComponent(typeof(AnimusAgent))]
    public class InteractableAgent : MonoBehaviour, IInteractable
    {
        private AnimusAgent Agent => GetComponent<AnimusAgent>();

        public string InteractionPrompt => $"Talk to {Agent.gameKey}";

        public void Interact(GameObject interactor)
        {
            var commandToExecute = $"/talk {Agent.gameKey} ";
            Chat.Instance.OpenChat(commandToExecute);
        }
    }
}