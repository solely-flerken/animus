using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Environment.PointOfInterest
{
    public class PointOfInterest : MonoBehaviour
    {
        private void Start()
        {
            PointOfInterestRegistry.Instance?.Register(this);
        }

        private void OnDisable()
        {
            PointOfInterestRegistry.Instance?.Unregister(this);
        }
    }
}