using Packages.Animus.Unity.Runtime.Core.Config.Script;

namespace Packages.Animus.Unity.Runtime.Core.Entity
{
    public class AnimusLocation : AnimusEntity
    {
        public override AnimusEntityType Type => AnimusEntityType.Location;

        private void OnEnable()
        {
            AnimusGameManager.EntityRegistry?.Register(this);
        }

        private void OnDisable()
        {
            AnimusGameManager.EntityRegistry?.Unregister(this);
        }
    }
}