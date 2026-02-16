using System.Collections.Generic;
using Features.Interaction.Scripts.Core;
using UnityEngine;

namespace Features.Gate.Scripts
{
    public class InteractableGate : MonoBehaviour, Interactable
    {
        public List<InteractionAction> Actions { get; } = new();
        
        [SerializeField] private Transform leftDoor;
        [SerializeField] private Transform rightDoor;
        [SerializeField] private float openAngle = 90f;
        [SerializeField] private float rotationSpeed = 3f;

        public bool IsOpen { get; private set; }
        
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
            
            Actions.Add(new DelegateAction(() => $"{(IsOpen ? "Close" : "Open")} Door", _ => ToggleDoors()));
        }

        private void Update()
        {
            _currentAngle = Mathf.Lerp(_currentAngle, _targetAngle, Time.deltaTime * rotationSpeed);

            leftDoor.localRotation = _leftClosedRotation * Quaternion.Euler(0, 0, -_currentAngle);
            rightDoor.localRotation = _rightClosedRotation * Quaternion.Euler(0, 0, _currentAngle);
        }

        public void ToggleDoors()
        {
            IsOpen = !IsOpen;
            _targetAngle = IsOpen ? openAngle : 0f;
        }

        public void OpenGate()
        {
            IsOpen = true;
            _targetAngle = openAngle;
        }

        public void CloseGate()
        {
            IsOpen = false;
            _targetAngle = 0f;
        }
    }
}