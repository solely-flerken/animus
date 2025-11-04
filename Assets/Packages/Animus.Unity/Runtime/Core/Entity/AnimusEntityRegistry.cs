using System.Linq;
using Packages.Animus.Unity.Runtime.Core.Utils;

namespace Packages.Animus.Unity.Runtime.Core.Entity
{
    public class AnimusEntityRegistry : TypeRegistry<AnimusEntityRegistry, AnimusEntity>
    {
        public T FindByGameKey<T>(string gameKey) where T : AnimusEntity
        {
            return string.IsNullOrEmpty(gameKey) ? null : GetAll<T>().FirstOrDefault(entity => entity.gameKey == gameKey);
        }
    }
}