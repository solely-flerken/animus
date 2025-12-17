using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Packages.Animus.Unity.Runtime.Core.Entity
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AnimusEntityType
    {
        None,
        Actor,
        Player,
        Agent,
        Object,
        Location
    }
}