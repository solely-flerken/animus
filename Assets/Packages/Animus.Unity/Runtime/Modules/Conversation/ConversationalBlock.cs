using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Core.Actions;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Modules.Conversation
{
    public class ConversationalBlock : MonoBehaviour
    {
        private const string TokenNotMyTurn = "Conversation_Wait";

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
            
            ActionQueueManager.Instance.ForceAgentThink(agentKey);
        }
    }
}