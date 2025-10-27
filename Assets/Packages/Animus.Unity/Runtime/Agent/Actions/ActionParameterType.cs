using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Converters;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ActionParameterType
    {
        String,
        Int,
        Float,
        Bool,
    }
}