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
        
        public int CurrentTurn { get; private set; }
        public static int MaxTurns => 10;
        public static int SoftEndTurn => 8;

        public Dictionary<string, string> ParticipantReasons { get; } = new();
        
        public string Id { get; } = Guid.NewGuid().ToString();
        public List<string> Participants { get; } = new();
        public string CurrentSpeakerKey { get; private set; }
        public DateTime LastInteractionTime { get; private set; }

        public ConversationAnchor(string initiatorKey, string targetKey, string initiatorReason = "small talk")
        {
            AddParticipant(initiatorKey, initiatorReason);
            AddParticipant(targetKey, "responding");
            
            // Initiator starts with the "Talking Stick"
            CurrentSpeakerKey = targetKey;
            LastInteractionTime = DateTime.UtcNow;
        }

        public void AddParticipant(string agentKey, string reason = "join conversation")
        {
            if (!Participants.Contains(agentKey))
            {
                // If they are already in another anchor, remove them from it first.
                if (ConversationAnchors.TryGetValue(agentKey, out var oldAnchor) && oldAnchor != this)
                {
                    oldAnchor.RemoveParticipant(agentKey);
                }

                Participants.Add(agentKey);
                ParticipantReasons[agentKey] = reason;
                ConversationAnchors[agentKey] = this;
                
                Participants.Sort(); 
            }
        }

        public void RemoveParticipant(string agentKey)
        {
            if (Participants.Contains(agentKey))
            {
                Participants.Remove(agentKey);
                ParticipantReasons.Remove(agentKey); 
                
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
            Debug.Log($"[ConversationAnchor] Conversation {Id} dissolved.");
        }
        
        public bool CheckStalemate()
        {
            var diff = DateTime.UtcNow - LastInteractionTime;
            
            if (diff.TotalSeconds > StalemateTimeoutSeconds)
            {
                Debug.Log($"[ConversationAnchor] Stalemate detected ({diff.TotalSeconds:F1}s silence). Kicking speaker: {CurrentSpeakerKey}");
                RemoveParticipant(CurrentSpeakerKey);
                return true;
            }
            
            return false;
        }
        
        public void PassTurn(string specificTargetKey = null)
        {
            LastInteractionTime = DateTime.UtcNow;
            CurrentTurn++;
            
            // Check if max turns reached
            if (CurrentTurn >= MaxTurns)
            {
                Debug.Log($"[ConversationAnchor] Max turns ({MaxTurns}) reached. Ending conversation.");
                Dissolve();
                return;
            }
            
            // If a specific target was addressed, and they are in the conversation, they go next.
            if (!string.IsNullOrEmpty(specificTargetKey) && Participants.Contains(specificTargetKey))
            {
                CurrentSpeakerKey = specificTargetKey;
                // Debug.Log($"[ConversationAnchor] Turn explicitly passed to {CurrentSpeakerKey}.");
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
            
            // Debug.Log($"[ConversationAnchor] Turn passed to next participant {CurrentSpeakerKey}.");
        }

        public bool IsAgentTurn(string agentKey)
        {
            return CurrentSpeakerKey == agentKey;
        }

        public bool CanContinueTalking()
        {
            return CurrentTurn < MaxTurns - 1;
        }

        public bool ShouldLeave()
        {
            return CurrentTurn >= MaxTurns - 1;
        }
        
        // TODO
        public string GetTurnContext(string agentKey)
        {
            var conversationReason = "participating in conversation";
            if (ParticipantReasons.TryGetValue(agentKey, out var reason))
            {
                conversationReason = reason;
            }

            var context = $"Turn {CurrentTurn}. Your reason for this conversation: {conversationReason}.";

            if (CurrentTurn >= MaxTurns - 1)
            {
                context += " This is the final exchange. Say goodbye and leave.";
            }
            else if (CurrentTurn >= SoftEndTurn)
            {
                context += " This conversation has gone on for a while. Consider wrapping up naturally if your goal is met.";
            }

            return context;
        }
    }
}