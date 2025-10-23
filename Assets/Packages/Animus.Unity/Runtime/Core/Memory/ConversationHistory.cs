using System.Collections.Generic;
using System.Linq;

namespace Packages.Animus.Unity.Runtime.Core.Memory
{
    internal class Conversation
    {
        private readonly List<DialogLine> _history = new();
        private readonly int _maxInMemoryLines;

        public int Count => _history.Count;

        public Conversation(int maxInMemoryLines)
        {
            _maxInMemoryLines = maxInMemoryLines;
        }

        public void AddLine(string speaker, string text)
        {
            var dialogueLine = new DialogLine(speaker, text);
            _history.Add(dialogueLine);

            // Trim the history if it exceeds the maximum size.
            if (_history.Count > _maxInMemoryLines)
            {
                _history.RemoveAt(0);
            }
        }

        public void Clear()
        {
            _history.Clear();
        }

        public List<DialogLine> GetRecentHistory(int lineCount)
        {
            return _history.TakeLast(lineCount).ToList();
        }
    }

    public class ConversationHistory
    {
        private readonly Dictionary<string, Conversation> _conversations;
        private readonly int _maxLinesPerConversation;

        public ConversationHistory(int maxLinesPerConversation = 100)
        {
            _conversations = new Dictionary<string, Conversation>();
            _maxLinesPerConversation = maxLinesPerConversation;
        }

        public void AddLine(string gameKey, string text)
        {
            if (!_conversations.ContainsKey(gameKey))
            {
                _conversations[gameKey] = new Conversation(_maxLinesPerConversation);
            }

            _conversations[gameKey].AddLine(gameKey, text);
        }

        public void ClearHistoryFor(string gameKey)
        {
            _conversations.Remove(gameKey);
        }

        public void ClearAllHistories()
        {
            _conversations.Clear();
        }

        public bool HasHistoryWith(string gameKey)
        {
            return _conversations.ContainsKey(gameKey);
        }

        public List<DialogLine> GetHistoryFor(string gameKey, int lineCount)
        {
            return HasHistoryWith(gameKey) ? _conversations[gameKey].GetRecentHistory(lineCount) : null;
        }
    }
}