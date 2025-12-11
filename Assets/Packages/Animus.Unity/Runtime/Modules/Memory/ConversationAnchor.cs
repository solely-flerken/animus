using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Modules.Memory
{
    public class ConversationAnchor
    {
        public static Dictionary<string, ConversationAnchor> ConversationAnchors = new();
        
        private const double StalemateTimeoutSeconds = 15.0f;
        
        public string Id { get; } = Guid.NewGuid().ToString();
        public List<string> Participants { get; } = new();
        public string CurrentSpeakerKey { get; private set; }
        public DateTime LastInteractionTime { get; private set; }

        public ConversationAnchor(string initiatorKey, string targetKey)
        {
            AddParticipant(initiatorKey);
            AddParticipant(targetKey);
            
            // Initiator starts with the "Talking Stick"
            CurrentSpeakerKey = targetKey;
            LastInteractionTime = DateTime.UtcNow;
        }

        public void AddParticipant(string agentKey)
        {
            if (!Participants.Contains(agentKey))
            {
                // If they are already in another anchor, remove them from it first.
                if (ConversationAnchors.TryGetValue(agentKey, out var oldAnchor) && oldAnchor != this)
                {
                    oldAnchor.RemoveParticipant(agentKey);
                }

                Participants.Add(agentKey);

                ConversationAnchors[agentKey] = this;
                
                Participants.Sort(); 
            }
        }

        public void RemoveParticipant(string agentKey)
        {
            if (Participants.Contains(agentKey))
            {
                Participants.Remove(agentKey);

                ConversationAnchors.Remove(agentKey);

                if (CurrentSpeakerKey == agentKey)
                {
                    PassTurn();
                }

                if (Participants.Count < 2)
                {
                    Dissolve();
                }
            }
        }
        
        private void Dissolve()
        {
            var currentParticipants = Participants.ToList();
            
            foreach (var p in currentParticipants)
            {
                ConversationAnchors.Remove(p);
            }
            
            Participants.Clear();
            Debug.Log($"[Anchor] Conversation {Id} dissolved.");
        }
        
        public bool CheckStalemate()
        {
            var diff = DateTime.UtcNow - LastInteractionTime;
            
            if (diff.TotalSeconds > StalemateTimeoutSeconds)
            {
                RemoveParticipant(CurrentSpeakerKey);
                Debug.Log($"[ConversationAnchor] Stalemate detected ({diff.TotalSeconds:F1}s silence). Kicking speaker: {CurrentSpeakerKey}");
                return true;
            }
            
            return false;
        }
        
        public void PassTurn(string specificTargetKey = null)
        {
            LastInteractionTime = DateTime.UtcNow;

            // If a specific target was addressed, and they are in the conversation, they go next.
            if (!string.IsNullOrEmpty(specificTargetKey) && Participants.Contains(specificTargetKey))
            {
                CurrentSpeakerKey = specificTargetKey;
                return;
            }
            
            Participants.Sort();
            var currentIndex = Participants.IndexOf(CurrentSpeakerKey);

            if (currentIndex != -1 && Participants.Count > 0)
            {
                var nextIndex = (currentIndex + 1) % Participants.Count;
                CurrentSpeakerKey = Participants[nextIndex];
            }
            else if (Participants.Count > 0)
            {
                // Fallback
                CurrentSpeakerKey = Participants.First();
            }
        }

        public bool IsAgentTurn(string agentKey)
        {
            return CurrentSpeakerKey == agentKey;
        }
    }
}