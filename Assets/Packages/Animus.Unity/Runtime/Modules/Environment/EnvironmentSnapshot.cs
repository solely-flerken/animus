using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Packages.Animus.Unity.Runtime.Core.Config.Script;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Infrastructure.Serialization.Converters;

namespace Packages.Animus.Unity.Runtime.Modules.Environment
{
    public class EnvironmentSnapshot
    {
        [JsonConverter(typeof(AnimusEntityListDetailsConverter))]
        public List<AnimusObject> VisibleObjects { get; set; }

        [JsonConverter(typeof(AnimusEntityListDetailsConverter))]
        public List<AnimusLocation> PointsOfInterest { get; set; } = AnimusGameManager.EntityRegistry.GetLocationsRelevantTo();

        [JsonConverter(typeof(AnimusEntityListDetailsConverter))]
        public List<AnimusActor> KnownCharacters => AnimusGameManager.EntityRegistry.GetAll<AnimusActor>().ToList();
    }
}