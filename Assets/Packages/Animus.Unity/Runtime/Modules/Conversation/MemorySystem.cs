using System;
using System.Collections.Generic;
using System.Linq;
using Packages.Animus.Unity.Runtime.Modules.GameTime;

namespace Packages.Animus.Unity.Runtime.Modules.Conversation
{
    [Serializable]
    public class MemorySystem
    {
        private List<Memory> _memories = new();
        private int _maxMemories;

        public MemorySystem(int maxMemories = 50)
        {
            _maxMemories = maxMemories;
        }

        public void AddMemory(string content)
        {
            var timeStr = TimeManager.Instance?.GetFormattedTime();
            var memory = new Memory(timeStr, content);
            _memories.Add(memory);

            // Keep only the most recent memories
            if (_memories.Count > _maxMemories)
            {
                _memories.RemoveAt(0);
            }
        }

        public List<string> GetFormattedMemories()
        {
            return _memories.Select(m => m.ToString()).ToList();
        }

        public List<Memory> GetMemories()
        {
            return new List<Memory>(_memories);
        }

        public void Clear()
        {
            _memories.Clear();
        }

        public int Count => _memories.Count;
    }
}