using Core.Input.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.Interaction.Scripts
{
    public class Interactor : MonoBehaviour
    {
        public Camera playerCamera;
        public LayerMask interactionLayerMask;
        public float interactionDistance = 5f;
        
        private static InputSystem_Actions.PlayerActions PlayerActions => InputManager.PlayerActions;

        private static InteractionUIManager InteractionUI => InteractionUIManager.Instance;

        private IInteractable _currentInteractable;

        private void Start()
        {
            PlayerActions.Enable();
            PlayerActions.Interact.performed += OnInteract;
        }

        private void OnDestroy()
        {
            PlayerActions.Interact.performed -= OnInteract;
            PlayerActions.Disable();
        }

        private void Update()
        {
            DetectInteractable();
        }

        private void OnInteract(InputAction.CallbackContext context)
        {
            _currentInteractable?.Interact(gameObject);
        }

        private void DetectInteractable()
        {
            var ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(ray, out var hit, interactionDistance, interactionLayerMask))
            {
                var interactable = hit.collider.GetComponentInParent<IInteractable>();

                if (interactable != null)
                {
                    if (interactable == _currentInteractable) return;
                    _currentInteractable = interactable;
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