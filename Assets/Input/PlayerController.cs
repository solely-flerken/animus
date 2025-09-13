using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour, InputSystem_Actions.IPlayerActions
    {
        [Header("Movement Settings")] public float moveSpeed = 5f;
        public float mouseSensitivity = 2f;
        public float jumpHeight = 2f;
        public float gravity = -9.81f;

        [Header("References")] public Transform cameraTransform;

        private CharacterController _controller;
        private InputSystem_Actions _inputActions;
        private InputSystem_Actions.PlayerActions _playerActions;

        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private float _verticalLookRotation;

        private Vector3 _velocity;
        private bool _isGrounded;

        #region Unity Callbacks

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();

            _inputActions = new InputSystem_Actions();
            _playerActions = _inputActions.Player;
            _playerActions.AddCallbacks(this);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnEnable()
        {
            _playerActions.Enable();
        }

        private void OnDisable()
        {
            _playerActions.Disable();
        }

        private void Update()
        {
            HandleMovement();
            HandleLook();
        }

        #endregion

        #region Private Methods

        private void HandleMovement()
        {
            // Ground check
            _isGrounded = _controller.isGrounded;
            if (_isGrounded && _velocity.y < 0)
                _velocity.y = -2f; // small downward push to keep grounded

            // Move input
            Vector3 move = transform.forward * _moveInput.y + transform.right * _moveInput.x;
            _controller.Move(move * (moveSpeed * Time.deltaTime));

            // Apply gravity + vertical velocity
            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }

        private void HandleLook()
        {
            // Horizontal rotation
            transform.Rotate(Vector3.up * (_lookInput.x * mouseSensitivity));

            // Vertical rotation with clamping
            _verticalLookRotation -= _lookInput.y * mouseSensitivity;
            _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, -90f, 90f);
            cameraTransform.localEulerAngles = new Vector3(_verticalLookRotation, 0, 0);
        }

        #endregion

        #region Input Callbacks

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            _lookInput = context.ReadValue<Vector2>();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && _isGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
        }

        public void OnNext(InputAction.CallbackContext context)
        {
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
        }

        #endregion
    }
}