using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Features.Interaction.Scripts
{
    [RequireComponent(typeof(AnimusObject))]
    public class InteractableObject : MonoBehaviour, IInteractable
    {
        public string InteractionPrompt => $"Pick up: {(_animusObject != null ? _animusObject.name : "")}";

        private AnimusObject _animusObject;

        private void Awake()
        {
            _animusObject = GetComponent<AnimusObject>();
        }

        public void Interact(GameObject interactor)
        {
            if (interactor.TryGetComponent<AnimusActor>(out var animusActor))
            {
                animusActor.inventory.AddItem(_animusObject.itemData, _animusObject.quantity);
                _animusObject.Pickup();
            }
        }
    }
}