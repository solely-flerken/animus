using Packages.Animus.Unity.Runtime.Core.Config.Script;

namespace Packages.Animus.Unity.Runtime.Core.Entity
{
    public class AnimusPlayer : AnimusActor
    {
        public override AnimusEntityType Type => AnimusEntityType.Player;

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