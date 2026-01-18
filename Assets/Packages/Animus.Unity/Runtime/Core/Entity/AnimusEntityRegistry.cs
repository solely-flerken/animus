using System;
using System.Collections.Generic;
using System.Linq;
using Packages.Animus.Unity.Runtime.Infrastructure.Utils;

namespace Packages.Animus.Unity.Runtime.Core.Entity
{
    public class AnimusEntityRegistry : TypeRegistry<AnimusEntityRegistry, AnimusEntity>
    {
        public T FindByGameKey<T>(string gameKey) where T : AnimusEntity
        {
            return string.IsNullOrEmpty(gameKey) ? null : GetAll<T>().FirstOrDefault(entity => entity.gameKey == gameKey);
        }
        
        public AnimusEntity FindByGameKey(string gameKey, Type type)
        {
            if (string.IsNullOrEmpty(gameKey) || type == null) return null;

            return allItems.FirstOrDefault(entity => entity.gameKey == gameKey && type.IsInstanceOfType(entity));
        }
        
        public List<AnimusLocation> GetLocationsRelevantTo(AnimusAgent agent = null)
        {
            var allLocations = GetAll<AnimusLocation>();
            
            if (!agent)
            {
                return allLocations.Where(loc => loc.contextScope == ContextScope.Global).ToList();
            }
    
            return allLocations.Where(location => location.IsRelevantTo(agent)).ToList();
        }
    }
}