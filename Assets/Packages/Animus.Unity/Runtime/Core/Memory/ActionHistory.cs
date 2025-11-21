using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Modules.Agent.Actions;

namespace Packages.Animus.Unity.Runtime.Core.Memory
{
    public class ActionHistory
    {
        private readonly List<ActionHistoryEntry> _history = new();

        public void AddEntry(ActionHistoryEntry entry)
        {
            _history.Add(entry);
        }

        public List<ActionHistoryEntry> GetHistory()
        {
            return _history;
        }

        public void Clear()
        {
            _history.Clear();
        }
    }
}