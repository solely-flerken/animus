using Packages.Animus.Unity.Runtime.Modules.Inventory;

namespace Packages.Animus.Unity.Runtime.Core.Entity
{
    public class AnimusActor : AnimusEntity
    {
        public override AnimusEntityType Type { get; } = AnimusEntityType.Actor;

        public Inventory inventory = new();
    }
}