using System;

namespace Packages.Animus.Unity.Runtime.Modules.Conversation
{
    [Serializable]
    public class Memory
    {
        public string timestamp;
        public string content;

        public Memory(string time, string memoryContent)
        {
            timestamp = time;
            content = memoryContent;
        }

        public override string ToString()
        {
            return $"[{timestamp}] {content}";
        }
    }
}