using System;
using Unity.Plastic.Newtonsoft.Json;

namespace Packages.Animus.Unity.Runtime.Core.Memory
{
    public readonly struct DialogLine
    {
        public string Speaker { get; }
        public string Text { get; }
        [JsonIgnore] public DateTime Timestamp { get; }
        
        public DialogLine(string speaker, string text)
        {
            Speaker = speaker;
            Text = text;
            Timestamp = DateTime.UtcNow;
        }
    }
}