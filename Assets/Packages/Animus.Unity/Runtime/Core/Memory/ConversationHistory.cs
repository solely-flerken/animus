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

        public List<DialogLine> GetFullHistory()
        {
            return new List<DialogLine>(_history);
        }
        
        public List<DialogLine> GetRecentHistory(int lineCount)
        {
            return _history.TakeLast(lineCount).ToList();
        }
        
        public void Clear()
        {
            _history.Clear();
        }
    }

    public class ConversationHistory
    {
        // Primary storage: Conversation Key -> Conversation Object
        private readonly Dictionary<string, Conversation> _conversations;
        
        // Index for fast lookups: Participant ID -> Set of Conversation Keys
        private readonly Dictionary<string, HashSet<string>> _participantIndex = new();
        
        private readonly int _maxLinesPerConversation;

        public ConversationHistory(int maxLinesPerConversation = 100)
        {
            _conversations = new Dictionary<string, Conversation>();
            _maxLinesPerConversation = maxLinesPerConversation;
        }

        /// <summary>
        /// Creates a unique, order-independent key from a list of participant IDs.
        /// </summary>
        public static string GetConversationKey(IEnumerable<string> participantIds)
        {
            // Use a sorted set to ensure uniqueness and order.
            var sortedIds = new SortedSet<string>(participantIds);
            return string.Join("_", sortedIds);
        }
        
        /// <summary>
        /// Adds a line of dialogue to a conversation.
        /// </summary>
        public void AddLine(List<string> participantIds, string speakerId, string text)
        {
            if (participantIds == null || participantIds.Count == 0) return;

            var conversationKey = GetConversationKey(participantIds);

            if (!_conversations.ContainsKey(conversationKey))
            {
                _conversations[conversationKey] = new Conversation(_maxLinesPerConversation);

                // If this is a new conversation, update the index for each participant.
                foreach (var participantId in participantIds)
                {
                    if (!_participantIndex.ContainsKey(participantId))
                    {
                        _participantIndex[participantId] = new HashSet<string>();
                    }
                    _participantIndex[participantId].Add(conversationKey);
                }
            }
            _conversations[conversationKey].AddLine(speakerId, text);
        }

        /// <summary>
        /// Retrieves history for conversations where all specified participants were present.
        /// This will find conversations that had more participants than specified.
        /// </summary>
        /// <param name="participantIds">The list of participants to find conversations for.</param>
        /// <param name="lineCount">The total number of recent lines to return across all found conversations.</param>
        /// <returns>A chronologically sorted list of dialogue lines.</returns>
        public List<DialogLine> GetHistoryFor(List<string> participantIds, int lineCount)
        {
            if (participantIds == null || participantIds.Count == 0)
            {
                return new List<DialogLine>();
            }

            // Find the set of conversations that all participants were a part of.
            HashSet<string> relevantKeys = null;
            foreach (var id in participantIds)
            {
                if (!_participantIndex.TryGetValue(id, out var participantConversations))
                {
                    return new List<DialogLine>(); // If one participant has no history, the intersection is empty.
                }

                if (relevantKeys == null)
                {
                    relevantKeys = new HashSet<string>(participantConversations);
                }
                else
                {
                    relevantKeys.IntersectWith(participantConversations);
                }
            }

            if (relevantKeys == null || relevantKeys.Count == 0)
            {
                return new List<DialogLine>();
            }

            // Collect all dialogue lines from the identified conversations.
            var combinedHistory = new List<DialogLine>();
            foreach (var key in relevantKeys)
            {
                // Use GetFullHistory to ensure correct sorting before taking the last N lines.
                combinedHistory.AddRange(_conversations[key].GetFullHistory());
            }

            // Sort all lines by time, take the most recent, and re-sort into chronological order.
            return combinedHistory
                .OrderByDescending(line => line.Timestamp)
                .Take(lineCount)
                .OrderBy(line => line.Timestamp)
                .ToList();
        }
        
        public void ClearAllHistories()
        {
            _conversations.Clear();
            _participantIndex.Clear();
        }
    }
}