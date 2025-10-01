using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Environment.PointOfInterest
{
    public class PointOfInterest : MonoBehaviour
    {
        private void Start()
        {
            PointOfInterestManager.Instance?.RegisterPoi(this);
        }

        private void OnDisable()
        {
            PointOfInterestManager.Instance?.UnregisterPoi(this);
        }
    }
}