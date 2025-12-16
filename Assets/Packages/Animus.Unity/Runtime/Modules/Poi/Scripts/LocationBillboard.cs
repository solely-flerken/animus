using Packages.Animus.Unity.Runtime.Core.Entity;
using TMPro;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Modules.Poi.Scripts
{
    [RequireComponent(typeof(AnimusLocation))]
    public class LocationBillboard : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Canvas targetCanvas;
        
        [SerializeField] private TextMeshProUGUI targetText;
        
        [Header("Distance Settings")]
        [Tooltip("Distance where the object is too close and is completely invisible.")]
        [SerializeField] private float fadeStartDistanceClose = 2f;

        [Tooltip("Distance where the object is close but fully visible.")]
        [SerializeField] private float fadeEndDistanceClose = 5f;

        [Tooltip("Distance where the object begins to fade out due to being too far.")]
        [SerializeField] private float fadeStartDistanceFar = 20f;

        [Tooltip("Distance where the object is completely invisible due to distance.")]
        [SerializeField] private float fadeEndDistanceFar = 50f;

        private Camera _mainCamera;
        private CanvasGroup _canvasGroup;
        private AnimusLocation _animusLocation;

        private void Start()
        {
            FindCamera();
            InitializeCanvas();
            InitializeText();
        }

        private void LateUpdate()
        {
            if (!_mainCamera) return;
            if (!targetCanvas || !_canvasGroup) return;

            UpdateBillboardRotation();
            UpdateFadeAlpha();
        }

        private void FindCamera()
        {
            _mainCamera = Camera.main;
            
            if (_mainCamera == null)
            {
                _mainCamera = FindFirstObjectByType<Camera>();
            }

            if (_mainCamera == null)
            {
                Debug.LogError("No camera found!");
            }
        }
        
        private void InitializeCanvas()
        {
            if (targetCanvas == null)
            {
                Debug.LogError($"[LocationBillboard] No target Canvas found on {name}.", this);
                return;
            }

            if (!targetCanvas.TryGetComponent(out _canvasGroup))
            {
                _canvasGroup = targetCanvas.gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void InitializeText()
        {
            if (targetText == null)
            {
                Debug.LogError($"[LocationBillboard] No TextMeshProUGUI component assigned on {name}.", this);
                return;
            }
            
            _animusLocation = GetComponent<AnimusLocation>();
            targetText.text = _animusLocation.entityName;
        }
        
        private void UpdateBillboardRotation()
        {
            // Face the camera
            var direction = targetCanvas.transform.position - _mainCamera.transform.position;
            targetCanvas.transform.rotation = Quaternion.LookRotation(direction);
        }

        private void UpdateFadeAlpha()
        {
            var locationPos = transform.position;
            var camPos = _mainCamera.transform.position;

            // Calculate horizontal distance (ignoring Y axis)
            var delta = new Vector3(locationPos.x - camPos.x, 0f, locationPos.z - camPos.z);
            var distance = delta.magnitude;

            // Calculate Fade In (Close range)
            var fadeInAlpha = Mathf.InverseLerp(fadeStartDistanceClose, fadeEndDistanceClose, distance);

            // Calculate Fade Out (Far range)
            var fadeOutProgress = Mathf.InverseLerp(fadeStartDistanceFar, fadeEndDistanceFar, distance);

            _canvasGroup.alpha = fadeInAlpha * (1f - fadeOutProgress);
        }
        
        private void OnValidate()
        {
            if (fadeEndDistanceClose < fadeStartDistanceClose) fadeEndDistanceClose = fadeStartDistanceClose;
            if (fadeStartDistanceFar < fadeEndDistanceClose) fadeStartDistanceFar = fadeEndDistanceClose;
            if (fadeEndDistanceFar < fadeStartDistanceFar) fadeEndDistanceFar = fadeStartDistanceFar;
        }
    }
}