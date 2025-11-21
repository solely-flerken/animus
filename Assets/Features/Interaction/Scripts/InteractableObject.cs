using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Features.Interaction.Scripts
{
    [RequireComponent(typeof(AnimusObject))]
    public class InteractableObject: MonoBehaviour, IInteractable
    {
        public AnimusObject AnimusObject => GetComponent<AnimusObject>();
        public string InteractionPrompt => $"Pick up: {AnimusObject.name}";
        public void Interact(GameObject interactor)
        {
            if (interactor.TryGetComponent<AnimusActor>(out var animusActor))
            {
                animusActor.inventory.AddItem(AnimusObject.itemData, AnimusObject.quantity);
                AnimusObject.Pickup();
            }
        }
    }
}