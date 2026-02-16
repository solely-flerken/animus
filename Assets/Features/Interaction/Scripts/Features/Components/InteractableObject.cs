using System.Collections.Generic;
using Features.Interaction.Scripts.Core;
using Features.Interaction.Scripts.Features.Actions;
using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Features.Interaction.Scripts.Features.Components
{
    [RequireComponent(typeof(AnimusObject))]
    public class InteractableObject : MonoBehaviour, Interactable
    {
        public List<InteractionAction> Actions { get; } = new();

        private AnimusObject AnimusObject => GetComponent<AnimusObject>();
        
        private void Awake()
        {
            Actions.Add(new PickUpAction(AnimusObject));
        }
    }
}