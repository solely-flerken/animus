using Core.Input.Scripts;
using Features.Interaction.Scripts.Core;
using Features.Interaction.Scripts.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.Interaction.Scripts.Features.Components
{
    public class Interactor : MonoBehaviour
    {
        public Camera playerCamera;
        public LayerMask interactionLayerMask;
        public float interactionDistance = 5f;
        
        private static InputSystem_Actions.PlayerActions PlayerActions => InputManager.PlayerActions;
        private static InteractionUIManager InteractionUI => InteractionUIManager.Instance;

        private Interactable _currentInteractable;

        private void Start()
        {
            PlayerActions.Enable();
            PlayerActions.Interact.performed += OnInteract;
            PlayerActions.Next.performed += OnNext;
            PlayerActions.Previous.performed += OnPrevious;
        }

        private void OnDestroy()
        {
            PlayerActions.Interact.performed -= OnInteract;
            PlayerActions.Next.performed -= OnNext;
            PlayerActions.Previous.performed -= OnPrevious;
            PlayerActions.Disable();
        }
        
        private void Update()
        {
            DetectInteractable();
        }

        private void OnInteract(InputAction.CallbackContext context)
        {
            var selectedAction = InteractionUI.GetSelectedAction();
            selectedAction?.Execute(gameObject);
        }

        private void OnNext(InputAction.CallbackContext obj)
        {
            InteractionUI.SelectNext();
        }

        private void OnPrevious(InputAction.CallbackContext obj)
        {
            InteractionUI.SelectPrevious();
        }
        
        private void DetectInteractable()
        {
            var ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(ray, out var hit, interactionDistance, interactionLayerMask))
            {
                var interactableActions = hit.collider.GetComponentInParent<Interactable>();

                if (interactableActions != null)
                {
                    if (interactableActions == _currentInteractable) return;
                    _currentInteractable = interactableActions;
                    InteractionUI.Show(_currentInteractable);
                }
                else
                {
                    // Looking at something, but it's not interactable.
                    _currentInteractable = null;
                    InteractionUI.Hide();
                }
            }
            else
            {
                // Looking at nothing.
                _currentInteractable = null;
                InteractionUI.Hide();
            }
        }
    }
}