using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Modules.Conversation
{
    public class ConversationAnchor
    {
        private const double StalemateTimeoutSeconds = 30.0f;
        
        public static event Action<List<string>, string> OnTurnChanged;
        public static event Action<string> OnParticipantLeft;
        public static event Action<List<string>> OnConversationEnded;
        
        public static readonly Dictionary<string, ConversationAnchor> ConversationAnchors = new();

        private string Id { get; } = Guid.NewGuid().ToString();
        public List<string> Participants { get; } = new();
        private string CurrentSpeakerKey { get; set; }
        private DateTime LastInteractionTime { get; set; }
        
        private int CurrentTurn { get; set; }
        private static int MaxTurns => 6;
        private static int SoftEndTurn => 4;

        /// <summary>
        /// Joins or creates a conversation between two participants.
        /// Returns the final anchor they are then both in.
        /// </summary>
        public static ConversationAnchor JoinOrCreate(string initiatorKey, string targetKey)
        {
            var anchors = ConversationAnchors;
            var sourceAnchor = anchors.GetValueOrDefault(initiatorKey);
            var targetAnchor = anchors.GetValueOrDefault(targetKey);
    
            ConversationAnchor finalAnchor;
    
            if (sourceAnchor != null && targetAnchor != null)
            {
                if (sourceAnchor == targetAnchor)
                {
                    // Both already in the same conversation
                    finalAnchor = sourceAnchor;
                }
                else
                {
                    // Both in different conversations, join the target's conversation
                    targetAnchor.AddParticipant(initiatorKey);
                    finalAnchor = targetAnchor;
                }
            }
            else if (sourceAnchor != null)
            {
                // Target isn't in a conversation, join the initiator's
                sourceAnchor.AddParticipant(targetKey);
                finalAnchor = sourceAnchor;
            }
            else if (targetAnchor != null)
            {
                // Initiator isn't in a conversation, join the target's
                targetAnchor.AddParticipant(initiatorKey);
                finalAnchor = targetAnchor;
            }
            else
            {
                // Both have no anchor
                finalAnchor = new ConversationAnchor(initiatorKey, targetKey);
            }
    
            return finalAnchor;
        }
        
        public static void CheckAllConversations()
        {
            // Use HashSet to automatically remove duplicates
            var uniqueAnchors = new HashSet<ConversationAnchor>(ConversationAnchors.Values);

            foreach (var anchor in uniqueAnchors)
            {
                anchor.CheckStalemate();
            }
        }
        
        private ConversationAnchor(string initiatorKey, string targetKey)
        {
            LastInteractionTime = DateTime.UtcNow;
            
            AddParticipant(initiatorKey);
            AddParticipant(targetKey);
            
            // Initiator starts with the "Talking Stick"
            CurrentSpeakerKey = initiatorKey;
            
            OnTurnChanged?.Invoke(Participants, CurrentSpeakerKey);
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
            if (!Participants.Contains(agentKey)) return;
            
            Participants.Remove(agentKey);
            ConversationAnchors.Remove(agentKey);
            
            OnParticipantLeft?.Invoke(agentKey);
            
            if (Participants.Count < 2)
            {
                Dissolve();
                return;
            }
            
            if (CurrentSpeakerKey == agentKey)
            {
                PassTurn();
            }
        }
        
        private void Dissolve()
        {
            if (Participants.Count == 0) return;
            
            foreach (var p in Participants.ToList())
            {
                ConversationAnchors.Remove(p);
            }
            
            OnConversationEnded?.Invoke(Participants);
            
            Participants.Clear();
            Debug.Log($"[ConversationAnchor] Conversation {Id} dissolved.");
        }
        
        private bool CheckStalemate()
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
            if (Participants.Count == 0) return;
            
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
            }
            else
            {
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
            
            OnTurnChanged?.Invoke(Participants, CurrentSpeakerKey);
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
        
        public string GetTurnContext()
        {
            var context = $"Turn {CurrentTurn}/{MaxTurns}.";

            if (CurrentTurn >= MaxTurns - 1)
            {
                context += " This is the final exchange. Say goodbye and leave.";
            }
            else if (CurrentTurn >= SoftEndTurn)
            {
                context += " This conversation has gone on for a while. Consider wrapping up naturally.";
            }

            return context;
        }
    }
}