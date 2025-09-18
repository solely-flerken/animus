using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    [System.Serializable]
    public struct MovementSpeeds
    {
        public float forwardSpeed;
        public float backwardSpeed;
        public float sidewaysSpeed;
    }

    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(InputManager))]
    public class PlayerController : MonoBehaviour, InputSystem_Actions.IPlayerActions
    {
        #region Variables

        [Header("Movement Settings")] public MovementSpeeds walkingSpeeds;
        public MovementSpeeds sprintingSpeeds;
        public float jumpHeight = 1f;
        public float gravityValue = -9.81f;

        [Header("Look")] public Transform cameraTransform;
        public float mouseSensitivity = 2f;
        public float verticalLookLimit = 80f;

        [Header("Animation")] public float animationSmoothTime = 0.1f;

        private CharacterController _controller;

        private InputManager _inputManager;
        private InputSystem_Actions.PlayerActions _playerActions;

        private Animator _animator;
        private static readonly int Forward = Animator.StringToHash("Forward");
        private static readonly int Right = Animator.StringToHash("Right");
        private static readonly int IsJumping = Animator.StringToHash("IsJumping");
        private static readonly int IsGrounded = Animator.StringToHash("IsGrounded");

        private Vector2 _moveInput;
        private Vector3 _playerVelocity;
        private Vector3 _currentVelocity;
        private Vector3 _velocitySmooth;

        private float _xRotation;
        private Vector2 _lookInput;

        private bool _sprintPressed;
        private bool _jumpPressed;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            _controller = GetComponent<CharacterController>();

            _inputManager = GetComponent<InputManager>();
            _playerActions = _inputManager.PlayerActions;
            _playerActions.AddCallbacks(this);
            _playerActions.Enable();

            _animator = GetComponentInChildren<Animator>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDestroy()
        {
            _playerActions.Disable();
        }

        private void Update()
        {
            HandleGravity();
            HandleMovement();
            UpdateAnimator();
        }

        private void LateUpdate()
        {
            HandleLook();
        }

        #endregion

        private void HandleGravity()
        {
            if (_controller.isGrounded && _playerVelocity.y < 0)
            {
                // Keep the player grounded
                _playerVelocity.y = -2f;
                _animator.SetBool(IsGrounded, true);
                _animator.SetBool(IsJumping, false);
            }

            if (_jumpPressed && _controller.isGrounded)
            {
                _playerVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravityValue);
                _animator.SetBool(IsJumping, true);
                _animator.SetBool(IsGrounded, false);
                _jumpPressed = false;
            }

            _playerVelocity.y += gravityValue * Time.deltaTime;
        }

        private void HandleMovement()
        {
            var speeds = _sprintPressed ? sprintingSpeeds : walkingSpeeds;

            var inputDir = new Vector3(_moveInput.x, 0, _moveInput.y);
            if (inputDir.sqrMagnitude > 1f) inputDir.Normalize();

            var targetVelocity = new Vector3(
                inputDir.x * speeds.sidewaysSpeed,
                0f,
                inputDir.z * (inputDir.z >= 0 ? speeds.forwardSpeed : speeds.backwardSpeed)
            );

            targetVelocity = transform.TransformDirection(targetVelocity);

            _currentVelocity =
                Vector3.SmoothDamp(_currentVelocity, targetVelocity, ref _velocitySmooth, 0.1f);
            _controller.Move((_currentVelocity + _playerVelocity) * Time.deltaTime);
        }

        private void HandleLook()
        {
            if (_lookInput.sqrMagnitude < 0.01f) return;

            var mouseX = _lookInput.x * mouseSensitivity;
            var mouseY = _lookInput.y * mouseSensitivity;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -verticalLookLimit, verticalLookLimit);

            cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }

        private void UpdateAnimator()
        {
            // Convert current velocity to local space
            var localVelocity = transform.InverseTransformDirection(_controller.velocity);

            // Forward/backward: divide by max forward/backward speed
            var forwardBlend = 0f;
            if (Mathf.Abs(localVelocity.z) > 0.01f)
            {
                forwardBlend = localVelocity.z > 0
                    ? localVelocity.z / sprintingSpeeds.forwardSpeed
                    : localVelocity.z / sprintingSpeeds.backwardSpeed;
            }

            // Right/left: divide by max sideways speed
            var rightBlend = 0f;
            if (Mathf.Abs(localVelocity.x) > 0.01f)
            {
                rightBlend = localVelocity.x / sprintingSpeeds.sidewaysSpeed;
            }

            // Clamp to [-1, 1]
            forwardBlend = Mathf.Clamp(forwardBlend, -1f, 1f);
            rightBlend = Mathf.Clamp(rightBlend, -1f, 1f);

            // Update animator with smooth interpolation
            _animator.SetFloat(Forward, forwardBlend, animationSmoothTime, Time.deltaTime);
            _animator.SetFloat(Right, rightBlend, animationSmoothTime, Time.deltaTime);
        }

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
            _jumpPressed = context.ReadValueAsButton();
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
        }

        public void OnNext(InputAction.CallbackContext context)
        {
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            _sprintPressed = context.ReadValueAsButton();
        }

        #endregion
    }
}