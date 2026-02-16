using Features.Interaction.Scripts.Core;
using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Features.Interaction.Scripts.Features.Actions
{
    public class PickUpAction : InteractionAction
    {
        private readonly AnimusObject _animusObject;
        
        public override string Label => $"Pick up: {(_animusObject != null ? _animusObject.name : "")}";
        
        public PickUpAction(AnimusObject animusObject)
        {
            _animusObject = animusObject;
        }
        
        public override void Execute(GameObject interactor)
        {
            if (interactor.TryGetComponent<AnimusActor>(out var animusActor))
            {
                animusActor.inventory.AddItem(_animusObject.itemData, _animusObject.quantity);
                _animusObject.Pickup();
            }
        }
    }
}