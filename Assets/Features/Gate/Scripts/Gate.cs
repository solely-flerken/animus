using Features.Interaction.Scripts;
using UnityEngine;

namespace Features.Gate.Scripts
{
    public class Gate : MonoBehaviour, IInteractable
    {
        public string InteractionPrompt => $"{(_isOpen ? "Close" : "Open")} Door";
        
        [SerializeField] private Transform leftDoor;
        [SerializeField] private Transform rightDoor;
        [SerializeField] private float openAngle = 90f;
        [SerializeField] private float rotationSpeed = 3f;

        private bool _isOpen;
        private float _targetAngle;
        private float _currentAngle;
        private Quaternion _leftClosedRotation;
        private Quaternion _rightClosedRotation;

        private void Start()
        {
            _leftClosedRotation = leftDoor.localRotation;
            _rightClosedRotation = rightDoor.localRotation;
            _currentAngle = 0f;
            _targetAngle = 0f;
        }

        private void Update()
        {
            _currentAngle = Mathf.Lerp(_currentAngle, _targetAngle, Time.deltaTime * rotationSpeed);

            leftDoor.localRotation = _leftClosedRotation * Quaternion.Euler(0, 0, -_currentAngle);
            rightDoor.localRotation = _rightClosedRotation * Quaternion.Euler(0, 0, _currentAngle);
        }

        public void ToggleDoors()
        {
            _isOpen = !_isOpen;
            _targetAngle = _isOpen ? openAngle : 0f;
        }

        public void OpenGate()
        {
            _isOpen = true;
            _targetAngle = openAngle;
        }

        public void CloseGate()
        {
            _isOpen = false;
            _targetAngle = 0f;
        }

        public void Interact(GameObject interactor)
        {
            ToggleDoors();
        }
    }
}