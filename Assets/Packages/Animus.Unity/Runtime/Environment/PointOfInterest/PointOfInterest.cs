using Packages.Animus.Unity.Runtime.Core.Entity;

namespace Packages.Animus.Unity.Runtime.Environment.PointOfInterest
{
    public class PointOfInterest : AnimusEntity
    {
        private void Start()
        {
            AnimusEntityRegistry.Instance.Register(this);
        }

        private void OnDisable()
        {
            AnimusEntityRegistry.Instance.Unregister(this);
        }
    }
}