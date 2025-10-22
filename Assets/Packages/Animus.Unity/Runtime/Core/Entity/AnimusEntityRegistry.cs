namespace Packages.Animus.Unity.Runtime.Core.Entity
{
    public class AnimusEntityRegistry : TypeRegistry<AnimusEntityRegistry, AnimusEntity>
    {
        public AnimusEntity FindByGameKey(string gameKey)
        {
            return allItems.Find(a => a.gameKey == gameKey);
        }
    }
}