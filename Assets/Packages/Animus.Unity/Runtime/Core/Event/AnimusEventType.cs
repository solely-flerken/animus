using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Packages.Animus.Unity.Runtime.Core.Event
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AnimusEventType
    {
        Dialog,
        Observation
    }
}