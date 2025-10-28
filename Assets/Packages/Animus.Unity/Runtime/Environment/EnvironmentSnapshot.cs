using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Core.Utils.Json;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Environment
{
    public class EnvironmentSnapshot
    {
        public List<object> VisibleObjects { get; set; }

        [JsonConverter(typeof(AnimusEntityListGameKeyConverter))]
        public List<PointOfInterest> PointsOfInterest => AnimusEntityRegistry.Instance.GetAll<PointOfInterest>();

        [JsonConverter(typeof(AnimusEntityListGameKeyConverter))]
        public List<AnimusEntity> KnownEntities => AnimusEntityRegistry.Instance.GetAll<AnimusEntity>();
    }
}