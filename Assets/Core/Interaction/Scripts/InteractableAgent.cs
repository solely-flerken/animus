
using Features.Chat.Scripts;
using Packages.Animus.Unity.Runtime.Agent;
using UnityEngine;

namespace Core.Interaction.Scripts
{
    [RequireComponent(typeof(Interactable), typeof(AnimusAgent))]
    public class InteractableAgent : MonoBehaviour
    {
        private Interactable Interactable =>  GetComponent<Interactable>();
        private AnimusAgent Agent => GetComponent<AnimusAgent>();
        
        private void Start()
        {
            Interactable.OnInteraction += Interact;
        }

        private void OnDestroy()
        {
            Interactable.OnInteraction -= Interact;
        }

        private void Interact(GameObject interactor)
        {
            var commandToExecute = $"/talk {Agent.gameKey} ";
            Chat.Instance.OpenChat(commandToExecute);
        }
    }
}