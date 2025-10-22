using Packages.Animus.Unity.Runtime.Core.Entity;

namespace Packages.Animus.Unity.Runtime.Player
{
    public class AnimusPlayer : AnimusEntity
    {
        public override AnimusEntityType Type => AnimusEntityType.Player;

        private void Start()
        {
            AnimusEntityRegistry.Instance.Register(this);
        }

        private void OnDisable()
        {
            AnimusEntityRegistry.Instance?.Unregister(this);
        }
    }
}