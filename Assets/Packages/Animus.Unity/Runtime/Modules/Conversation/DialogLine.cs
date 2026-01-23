using System;
using Newtonsoft.Json;
using Packages.Animus.Unity.Runtime.Modules.GameTime;

namespace Packages.Animus.Unity.Runtime.Modules.Conversation
{
    public readonly struct DialogLine
    {
        public string Speaker { get; }
        public string Text { get; }
        public string GameTime { get; } 
        
        [JsonIgnore] public DateTime Timestamp { get; }
        
        public DialogLine(string speaker, string text)
        {
            Speaker = speaker;
            Text = text;
            GameTime = TimeManager.Instance?.GetFormattedTime();
            Timestamp = DateTime.UtcNow;
        }
        
        public override string ToString()
        {
            return $"[{GameTime}] {Speaker}: {Text}";
        }
    }
}