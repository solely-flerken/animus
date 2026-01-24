using System.Collections.Generic;
using System.Linq;
using Packages.Animus.Unity.Runtime.Core.Actions;
using Packages.Animus.Unity.Runtime.Core.Config.Script;
using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Modules.Conversation
{
    public class ConversationManager : MonoBehaviour
    {
        private static ConversationManager Instance { get; set; }

        private const float CheckInterval = 1.0f;
        private float _timer;

        private const string TokenNotMyTurn = "Conversation_Wait";
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void OnEnable()
        {
            ConversationAnchor.OnTurnChanged += HandleTurnChanged;
            ConversationAnchor.OnConversationEnded += HandleConversationEnded;
            ConversationAnchor.OnParticipantJoin += HandleParticipantJoin;
            ConversationAnchor.OnParticipantLeft += HandleParticipantLeft;
        }

        private void OnDisable()
        {
            ConversationAnchor.OnTurnChanged -= HandleTurnChanged;
            ConversationAnchor.OnConversationEnded -= HandleConversationEnded;
            ConversationAnchor.OnParticipantJoin -= HandleParticipantJoin;
            ConversationAnchor.OnParticipantLeft -= HandleParticipantLeft;
        }
        
        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= CheckInterval)
            {
                _timer = 0;
                ConversationAnchor.CheckAllConversations();
            }
        }
        
        private static void HandleTurnChanged(List<string> participants, string currentSpeaker)
        {
            if (ActionQueueManager.Instance == null) return;

            foreach (var agentKey in participants)
            {
                if (agentKey == currentSpeaker)
                {
                    ActionQueueManager.Instance.RemoveBlockToken(agentKey, TokenNotMyTurn);
                }
                else
                {
                    ActionQueueManager.Instance.AddBlockToken(agentKey, TokenNotMyTurn);
                }
            }
        }

        private static void HandleConversationEnded(List<string> participants)
        {
            if (ActionQueueManager.Instance == null) return;
            
            foreach (var agentKey in participants)
            {
                var agent = AnimusGameManager.EntityRegistry.FindByGameKey<AnimusAgent>(agentKey);
                
                if (agent != null)
                {
                    var others = string.Join(", ", participants.Where(p => p != agentKey));
                    agent.memorySystem.AddMemory($"Ended conversation with {others}.");
                }
             
                ActionQueueManager.Instance.RemoveBlockToken(agentKey, TokenNotMyTurn);
            }
        }
        
        private static void HandleParticipantJoin(List<string> participants, string agentKey)
        {
            var agent = AnimusGameManager.EntityRegistry.FindByGameKey<AnimusAgent>(agentKey);
            
            if (agent == null) return;
            
            if (participants.Count == 2)
            {
                foreach (var participantKey in participants)
                {
                    var participant = AnimusGameManager.EntityRegistry.FindByGameKey<AnimusAgent>(participantKey);
                    
                    if (participant == null) continue;
                    
                    var others = string.Join(", ", participants.Where(p => p != participantKey));
                    participant.memorySystem.AddMemory($"Started a conversation with {others}...");
                }
            }
            else if (participants.Count > 2)
            {
                // Someone joined an ongoing conversation
                var others = string.Join(", ", participants.Where(p => p != agentKey));
                agent.memorySystem.AddMemory($"Joined conversation with {others}...");
            }
        }
        
        private static void HandleParticipantLeft(string agentKey)
        {
            if (ActionQueueManager.Instance == null) return;
            
            var agent = AnimusGameManager.EntityRegistry.FindByGameKey<AnimusAgent>(agentKey);
            agent?.memorySystem.AddMemory("Left the conversation.");
            
            ActionQueueManager.Instance.RemoveBlockToken(agentKey, TokenNotMyTurn);
            ActionQueueManager.Instance.ForceAgentThink(agentKey);
        }
    }
}