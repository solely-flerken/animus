using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Packages.Animus.Unity.Runtime.Core.Memory
{
    public class Conversation
    {
        private readonly List<DialogLine> _history = new();
        private readonly int _maxInMemoryLines;

        public IReadOnlyList<DialogLine> History => _history;

        public Conversation(int maxInMemoryLines = 20)
        {
            _maxInMemoryLines = maxInMemoryLines;
        }

        public void AddLine(string speaker, string text)
        {
            AddLine(new DialogLine(speaker, text));
        }

        public void AddLine(DialogLine line)
        {
            _history.Add(line);

            if (_history.Count > _maxInMemoryLines)
            {
                var toRemove = _history.Count - _maxInMemoryLines;
                _history.RemoveRange(0, toRemove);
            }
        }

        public IReadOnlyList<DialogLine> GetRecentHistory(int lineCount)
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
        private readonly ConcurrentDictionary<string, Lazy<Conversation>> _conversations = new();

        // Index: Participant ID -> Set of Conversation Keys they're in
        private readonly ConcurrentDictionary<string, HashSet<string>> _participantIndex = new();

        private readonly object _indexLock = new object();

        private readonly int _maxLinesPerConversation;

        public ConversationHistory(int maxLinesPerConversation = 100)
        {
            _maxLinesPerConversation = maxLinesPerConversation;
        }

        /// <summary>
        /// Creates a unique, order-independent key from a list of participant IDs.
        /// </summary>
        public static string GetConversationKey(IEnumerable<string> participantIds)
        {
            return string.Join("|", participantIds.OrderBy(id => id, StringComparer.Ordinal));
        }

        /// <summary>
        /// Adds a line of dialogue to a conversation.
        /// </summary>
        public void AddLine(List<string> participantIds, string speakerId, string text)
        {
            var conversationKey = GetConversationKey(participantIds);

            var conversationLazy = _conversations.GetOrAdd(conversationKey,
                _ => new Lazy<Conversation>(() => new Conversation(_maxLinesPerConversation)));

            conversationLazy.Value.AddLine(speakerId, text);

            // Update the participant index
            lock (_indexLock)
            {
                foreach (var participantId in participantIds)
                {
                    if (!_participantIndex.TryGetValue(participantId, out var conversationKeys))
                    {
                        conversationKeys = new HashSet<string>(StringComparer.Ordinal);
                        _participantIndex[participantId] = conversationKeys;
                    }

                    conversationKeys.Add(conversationKey);
                }
            }
        }

        /// <summary>
        /// Gets conversation history for the specified participants.
        /// Returns all conversations where ALL specified participants were present, even if additional participants were also involved.
        /// </summary>
        public List<DialogLine> GetHistoryFor(HashSet<string> participantIds, int lineCount)
        {
            if (participantIds is null || participantIds.Count == 0)
            {
                return new List<DialogLine>();
            }

            // Find the intersection of conversation keys that all participants share
            HashSet<string> matchingConversationKeys;

            lock (_indexLock)
            {
                var allParticipantsKnown = participantIds.All(_participantIndex.ContainsKey);
                if (!allParticipantsKnown)
                {
                    // If any participant has no conversations, return empty
                    return new List<DialogLine>();
                }

                matchingConversationKeys = participantIds
                    .Select(id => _participantIndex[id])
                    .Aggregate((current, next) =>
                    {
                        var intersection = new HashSet<string>(current, StringComparer.Ordinal);
                        intersection.IntersectWith(next);
                        return intersection;
                    });
            }

            // Collect all matching lines from the matching conversations
            var allMatchingLines = new List<DialogLine>();

            foreach (var conversationKey in matchingConversationKeys)
            {
                if (_conversations.TryGetValue(conversationKey, out var conversation))
                {
                    allMatchingLines.AddRange(conversation.Value.History);
                }
            }

            return allMatchingLines.TakeLast(lineCount).ToList();
        }

        public void ClearAllHistories()
        {
            _conversations.Clear();
            lock (_indexLock)
            {
                _participantIndex.Clear();
            }
        }
    }
}