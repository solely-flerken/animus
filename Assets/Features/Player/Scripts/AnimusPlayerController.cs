using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Core.Actions;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Modules.Conversation;
using UnityEngine;

namespace Features.Player.Scripts
{
    [RequireComponent(typeof(AnimusPlayer))]
    public class AnimusPlayerController : MonoBehaviour
    {
        private AnimusPlayer _player;
        
        private void Awake()
        {
            _player = GetComponent<AnimusPlayer>();
        }

        private void OnEnable()
        {
            Chat.Scripts.Chat.OnChatClosed += HandleChatClosed;
        }

        private void OnDisable()
        {
            Chat.Scripts.Chat.OnChatClosed -= HandleChatClosed;
        }

        private void HandleChatClosed()
        {
            var anchor = ConversationAnchor.ConversationAnchors.GetValueOrDefault(_player.gameKey);
            
            if(anchor == null) return;
            
            if (!anchor.IsAgentTurn(_player.gameKey)) return;
            
            Debug.Log("[Chat] Player canceled his turn. Passing to next participant.");
            anchor.PassTurn();
        }

        public void InitiateConversation(string targetActorKey)
        {
            // Cancel every participant's actions
            if (ConversationAnchor.ConversationAnchors.TryGetValue(targetActorKey, out var currentTargetAnchor))
            {
                currentTargetAnchor.Participants.ForEach(p => ActionQueueManager.Instance?.CancelAgentRequest(p));
            }
            
            var anchors = ConversationAnchor.ConversationAnchors;
            var sourceAnchor = anchors.GetValueOrDefault(_player.gameKey);
            var targetAnchor = anchors.GetValueOrDefault(targetActorKey);
            
            ConversationAnchor finalAnchor;
            
            if (sourceAnchor != null && targetAnchor != null)
            {
                if (sourceAnchor == targetAnchor)
                {
                    // Both already in the same conversation
                }
                else
                {
                    // Both in different conversations, join the target's conversation
                    targetAnchor.AddParticipant(_player.gameKey);
                }

                finalAnchor = targetAnchor;
            }
            else if (sourceAnchor != null)
            {
                // Target isn't in a conversation, join the initiator's
                sourceAnchor.AddParticipant(targetActorKey);
                finalAnchor = sourceAnchor;
            }
            else if (targetAnchor != null)
            {
                // Initiator isn't in a conversation, join the target's
                targetAnchor.AddParticipant(_player.gameKey);
                finalAnchor = targetAnchor;
            }
            else
            {
                // Both have no anchor
                finalAnchor = new ConversationAnchor(_player.gameKey, targetActorKey);
            }
            
            // Pass the turn to the player (stops other participants from talking)
            finalAnchor.PassTurn(_player.gameKey);
            
            var commandToExecute = $"/talk {targetActorKey} ";
            Chat.Scripts.Chat.Instance.OpenChat(commandToExecute);
        }
        
        public void PlayerSpeak(string message, string targetActorKey)
        {
            var anchor = ConversationAnchor.ConversationAnchors.GetValueOrDefault(_player.gameKey);
            AnimusAgent.SharedHistory.AddLine(new List<string>(anchor.Participants), _player.gameKey, message);
            anchor.PassTurn(targetActorKey);
        }
    }
}