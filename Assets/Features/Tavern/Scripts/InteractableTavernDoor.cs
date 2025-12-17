using Features.Interaction.Scripts;
using UnityEngine;

namespace Features.Tavern.Scripts
{
    public class InteractableTavernDoor : MonoBehaviour, IInteractable
    {
        public string InteractionPrompt => $"{(_isOpen ? "Close" : "Open")} Door";
        
        [SerializeField] private Transform door;
        [SerializeField] private float openAngle = 90f;
        [SerializeField] private float rotationSpeed = 3f;
        
        private bool _isOpen;
        private float _targetAngle;
        private float _currentAngle;
        private Quaternion _closedRotation;
        
        private int _entitiesInRange;
        
        private void Start()
        {
            _closedRotation = door.localRotation;
            _currentAngle = 0f;
            _targetAngle = 0f;
        }
        
        private void Update()
        {
            _currentAngle = Mathf.Lerp(_currentAngle, _targetAngle, Time.deltaTime * rotationSpeed);
            door.localRotation = _closedRotation * Quaternion.Euler(0, _currentAngle, 0);
        }

        private void OnTriggerEnter(Collider other)
        {
            _entitiesInRange++;
            
            if (!_isOpen)
            {
                OpenDoor(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            _entitiesInRange--;

            if (_entitiesInRange <= 0 && _isOpen)
            {
                _entitiesInRange = 0;
                CloseGate();
            }
        }

        private void OpenDoor(GameObject entity)
        {
            // Determine which side the entity is on
            var doorForward = door.forward;
            var toEntity = (entity.transform.position - door.position).normalized;
            var dot = Vector3.Dot(doorForward, toEntity);
            
            _isOpen = true;
            _targetAngle = dot > 0 ? -openAngle : openAngle;
        }
        
        private void CloseGate()
        {
            _isOpen = false;
            _targetAngle = 0f;
        }
        
        public void Interact(GameObject interactor)
        {
            if (_isOpen)
            {
                _isOpen = false;
                _targetAngle = 0f;
            }
            else
            {
                OpenDoor(interactor);
            }
        }
    }
}