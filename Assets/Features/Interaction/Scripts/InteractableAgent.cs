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
            Brain.StartGoalIdle();
            
            var player = interactor.GetComponent<AnimusPlayerController>();
            player?.InitiateConversation(Agent.gameKey);
        }
    }
}