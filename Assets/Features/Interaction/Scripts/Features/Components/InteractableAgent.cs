using System.Collections.Generic;
using Features.Interaction.Scripts.Core;
using Features.Interaction.Scripts.Features.Actions;
using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Features.Interaction.Scripts.Features.Components
{
    [RequireComponent(typeof(AnimusAgent))]
    public class InteractableAgent : MonoBehaviour, Interactable
    {
        public List<InteractionAction> Actions { get; } = new();
        
        private AnimusAgent Agent => GetComponent<AnimusAgent>();
        
        private void Awake()
        {
            Actions.Add(new TalkAction(Agent.gameKey));
        }
    }
}