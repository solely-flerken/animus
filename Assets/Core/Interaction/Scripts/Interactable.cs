using System;
using UnityEngine;

namespace Core.Interaction.Scripts
{
    public class Interactable : MonoBehaviour
    {
        public string interactionTypeName = "Interact";

        public event Action<GameObject> OnInteraction;

        public void InvokeInteraction(GameObject interactor)
        {
            Debug.Log($"{interactor.name} interacted with {name}");
            OnInteraction?.Invoke(interactor);
        }
    }
}