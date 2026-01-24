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
            var playerKey = _player.gameKey;
            
            var alreadyTalking = ConversationAnchor.ConversationAnchors.TryGetValue(targetActorKey, out var anchor) && anchor.Participants.Contains(playerKey);

            if (!alreadyTalking)
            {
                // TODO: Maybe set status to smth like: "You got interrupted by X."
                ActionQueueManager.InterruptAgent(targetActorKey);
                Debug.Log($"[Debug] {targetActorKey}: Action interrupted through 'InitiateConversation'.");
            }
            
            var finalAnchor = ConversationAnchor.JoinOrCreate(_player.gameKey, targetActorKey);
            
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