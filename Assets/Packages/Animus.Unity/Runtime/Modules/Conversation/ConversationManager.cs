using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Core.Actions;
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
            ConversationAnchor.OnParticipantLeft += HandleParticipantLeft;
        }

        private void OnDisable()
        {
            ConversationAnchor.OnTurnChanged -= HandleTurnChanged;
            ConversationAnchor.OnConversationEnded -= HandleConversationEnded;
            ConversationAnchor.OnParticipantLeft -= HandleParticipantLeft;
        }
        
        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= CheckInterval)
            {
                _timer = 0;
                CheckAllConversations();
            }
        }

        private static void CheckAllConversations()
        {
            var activeAnchors = new List<ConversationAnchor>(ConversationAnchor.ConversationAnchors.Values);

            foreach (var anchor in activeAnchors)
            {
                anchor.CheckStalemate();
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
                ActionQueueManager.Instance.RemoveBlockToken(agentKey, TokenNotMyTurn);
            }
        }
        
        private static void HandleParticipantLeft(string agentKey)
        {
            if (ActionQueueManager.Instance == null) return;
            
            ActionQueueManager.Instance.RemoveBlockToken(agentKey, TokenNotMyTurn);
            ActionQueueManager.Instance.ForceAgentThink(agentKey);
        }
    }
}