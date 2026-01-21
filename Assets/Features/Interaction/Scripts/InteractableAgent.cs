using Features.Goap.Agents;
using Features.Player.Scripts;
using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Features.Interaction.Scripts
{
    [RequireComponent(typeof(AnimusAgent), typeof(SimpleAgentBrain))]
    public class InteractableAgent : MonoBehaviour, IInteractable
    {
        private AnimusAgent Agent => GetComponent<AnimusAgent>();
        private SimpleAgentBrain Brain => GetComponent<SimpleAgentBrain>();

        public string InteractionPrompt => $"Talk to {Agent.gameKey}";

        public void Interact(GameObject interactor)
        {
            // TODO: Maybe set status to smth like: "You got interrupted by X."
            Agent.actionStatus.Set("None.");
            Brain.StartGoalIdle();
            
            var player = interactor.GetComponent<AnimusPlayerController>();
            player?.InitiateConversation(Agent.gameKey);
        }
    }
}