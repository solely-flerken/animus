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