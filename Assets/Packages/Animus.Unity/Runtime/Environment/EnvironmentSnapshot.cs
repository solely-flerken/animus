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
     * TODO: Make it so each field in here gets a type e.g. PointsOfInterest get the Location type.
     * That way we can use those types to define the types of actions parameters.
     * Also we need to refactor the ActionParameterType to include those types.
     */
    public class EnvironmentSnapshot
    {
        public List<object> VisibleObjects { get; set; }

        [JsonConverter(typeof(AnimusEntityListGameKeyConverter<AnimusLocation>))]
        public List<AnimusLocation> PointsOfInterest => AnimusEntityRegistry.Instance.GetAll<AnimusLocation>();

        [JsonConverter(typeof(AnimusEntityListGameKeyConverter<AnimusEntity>))]
        public List<AnimusEntity> KnownEntities =>
            AnimusEntityRegistry.Instance
                .GetAll<AnimusEntity>()
                .Where(e => e is AnimusAgent or AnimusPlayer)
                .ToList();
    }
}