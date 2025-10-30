using System.Collections.Generic;
using System.Linq;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Core.Utils.Json;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Environment
{
    public class EnvironmentSnapshot
    {
        [JsonConverter(typeof(AnimusEntityListDetailsConverter))]
        public List<AnimusEntity> VisibleObjects { get; set; }

        [JsonConverter(typeof(AnimusEntityListDetailsConverter))]
        public List<AnimusLocation> PointsOfInterest => AnimusEntityRegistry.Instance.GetAll<AnimusLocation>();

        [JsonConverter(typeof(AnimusEntityListDetailsConverter))]
        public List<AnimusActor> KnownEntities => AnimusEntityRegistry.Instance.GetAll<AnimusActor>().ToList();
    }
}