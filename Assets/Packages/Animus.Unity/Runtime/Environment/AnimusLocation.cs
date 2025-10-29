using Packages.Animus.Unity.Runtime.Core.Entity;

namespace Packages.Animus.Unity.Runtime.Environment
{
    public class AnimusLocation : AnimusEntity
    {
        public override AnimusEntityType Type => AnimusEntityType.Location;

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