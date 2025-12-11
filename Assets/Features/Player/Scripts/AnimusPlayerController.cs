using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Core.Actions;
using Packages.Animus.Unity.Runtime.Core.Config.Script;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Modules.Memory;
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

        public void PlayerSpeak(string message, string targetActorKey)
        {
            if (string.IsNullOrEmpty(message)) return;

            var targetActor = AnimusGameManager.EntityRegistry.FindByGameKey<AnimusActor>(targetActorKey);
            if (targetActor == null)
            {
                return;
            }
            
            // Since we interact with a certain agent that agent's context is now outdated
            ActionQueueManager.Instance?.CancelAgentRequest(targetActorKey);
            
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
                    // Both in different conversations, "kidnap" the target from its conversation
                    sourceAnchor.AddParticipant(targetActorKey);
                }

                finalAnchor = sourceAnchor;
            }
            else if (sourceAnchor != null)
            {
                sourceAnchor.AddParticipant(targetActorKey);
                finalAnchor = sourceAnchor;
            }
            else if (targetAnchor != null)
            {
                targetAnchor.AddParticipant(_player.gameKey);
                finalAnchor = targetAnchor;
            }
            else
            {
                // Both have no anchor
                finalAnchor = new ConversationAnchor(_player.gameKey, targetActorKey);
            }

            AnimusAgent.SharedHistory.AddLine(new List<string>(finalAnchor.Participants), _player.gameKey, message);
            finalAnchor.PassTurn(targetActorKey);
        }
    }
}