using System.Collections.Generic;
using System.Linq;
using Packages.Animus.Unity.Runtime.Agent;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Core.Utils.Json;
using Packages.Animus.Unity.Runtime.Player;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Environment
{
    public class EnvironmentSnapshot
    {
        public List<object> VisibleObjects { get; set; }

        [JsonConverter(typeof(AnimusEntityListGameKeyConverter<PointOfInterest>))]
        public List<PointOfInterest> PointsOfInterest => AnimusEntityRegistry.Instance.GetAll<PointOfInterest>();

        [JsonConverter(typeof(AnimusEntityListGameKeyConverter<AnimusEntity>))]
        public List<AnimusEntity> KnownEntities =>
            AnimusEntityRegistry.Instance
                .GetAll<AnimusEntity>()
                .Where(e => e is AnimusAgent or AnimusPlayer)
                .ToList();
    }
}