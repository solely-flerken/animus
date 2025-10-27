using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Converters;

namespace Packages.Animus.Unity.Runtime.Core.Event
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AnimusEventType
    {
        Dialog,
        Observation
    }
}