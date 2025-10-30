using System.Collections.Generic;
using System.Linq;
using Packages.Animus.Unity.Runtime.Agent;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Core.Utils.Json;
using Packages.Animus.Unity.Runtime.Player;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Environment
{
    /*
     * TODO: Add types to environment descriptions.
     * Add AnimusObjects
     * Add description, ... fields to entity in environment descriptions
     * Refactor/Fix JsonConverters
     */
    public class EnvironmentSnapshot
    {
        [JsonConverter(typeof(AnimusEntityListDetailsConverter))]
        public List<AnimusEntity> VisibleObjects { get; set; }

        [JsonConverter(typeof(AnimusEntityListDetailsConverter))]
        public List<AnimusLocation> PointsOfInterest => AnimusEntityRegistry.Instance.GetAll<AnimusLocation>();

        [JsonConverter(typeof(AnimusEntityListDetailsConverter))]
        public List<AnimusEntity> KnownEntities =>
            AnimusEntityRegistry.Instance
                .GetAll<AnimusEntity>()
                .Where(e => e is AnimusAgent or AnimusPlayer)
                .ToList();
    }
}