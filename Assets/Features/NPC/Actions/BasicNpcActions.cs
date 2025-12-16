using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Features.Goap.Agents;
using Packages.Animus.Unity.Runtime.Core.Actions;
using Packages.Animus.Unity.Runtime.Core.Config.Script;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Modules.Memory;
using UnityEngine;

namespace Features.NPC.Actions
{
    [RequireComponent(typeof(AnimusAgent), typeof(SimpleAgentBrain))]
    public class BasicNpcActions : MonoBehaviour
    {
        private AnimusAgent _agent;
        private SimpleAgentBrain _brain;

        private void Awake()
        {
            _agent = GetComponent<AnimusAgent>();
            _brain = GetComponent<SimpleAgentBrain>();
        }

        [AgentAction("idle", "Remain idle and take no action.")]
        public UniTask<string> Idle()
        {
            return UniTask.FromResult("I stood idle for a moment.");
        }
        
        [AgentAction("move_to", "Moves the agent to a specific entity.")]
        public UniTask<string> MoveTo(string entityKey)
        {
            if (ConversationAnchor.ConversationAnchors.TryGetValue(_agent.gameKey, out var anchor))
            {
                anchor.RemoveParticipant(_agent.gameKey);
            }
            
            var targetEntity = AnimusGameManager.EntityRegistry.FindByGameKey<AnimusEntity>(entityKey);
            if (targetEntity == null)
            {
                return UniTask.FromResult($"I tried to move to '{entityKey}', but I couldn't find it.");
            }

            // TODO: Maybe callback to add to memory that the agent arrived at his target
            _brain.StartGoalMoveTo(targetEntity);

            return UniTask.FromResult($"I started moving towards {targetEntity.gameKey}.");
        }

        [AgentAction("talk", "Say something to another character.")]
        public UniTask<string> Talk(string message, string targetActorKey)
        {
            var targetActor = AnimusGameManager.EntityRegistry.FindByGameKey<AnimusActor>(targetActorKey);
            if (targetActor == null)
            {
                return UniTask.FromResult("");
            }
            
            // Since we interact with a certain agent that agent's context is now outdated
            ActionQueueManager.Instance?.CancelAgentRequest(targetActorKey);
            
            // Cancel every participant's actions
            if (ConversationAnchor.ConversationAnchors.TryGetValue(targetActorKey, out var currentTargetAnchor))
            {
                currentTargetAnchor.Participants.ForEach(p => ActionQueueManager.Instance?.CancelAgentRequest(p));
            }
            
            var anchors = ConversationAnchor.ConversationAnchors;
            var sourceAnchor = anchors.GetValueOrDefault(_agent.gameKey);
            var targetAnchor = anchors.GetValueOrDefault(targetActorKey);
            var hasSourceAnchor = sourceAnchor != null;
            var hasTargetAnchor = targetAnchor != null;
            
            if (hasSourceAnchor && hasTargetAnchor)
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

                sourceAnchor.PassTurn(targetActorKey);
            }
            else if (hasSourceAnchor)
            {
                sourceAnchor.AddParticipant(targetActorKey);
                sourceAnchor.PassTurn(targetActorKey);
            }
            else if (hasTargetAnchor)
            {
                targetAnchor.AddParticipant(_agent.gameKey);
                targetAnchor.PassTurn(targetActorKey);
            }
            else
            {
                // Both have no anchor
                _ = new ConversationAnchor(_agent.gameKey, targetActorKey);
            }

            if (ConversationAnchor.ConversationAnchors.TryGetValue(_agent.gameKey, out var finalAnchor))
            {
                AnimusAgent.SharedHistory.AddLine(new List<string>(finalAnchor.Participants), _agent.gameKey, message);
            }
            else
            {
                Debug.LogError($"[BasicNpcActions] Critical Error: Anchor not found for {_agent.gameKey} after creation. This shouldn't be possible.");
            }
            
            _brain.StartGoalTalk(message, targetActor);

            return UniTask.FromResult($"I said '{message}' to {targetActor.gameKey}.");
        }
        
        [AgentAction("leave_conversation", "End the conversation with a final message.")]
        public UniTask<string> LeaveConversation(string finalMessage, string targetActorKey)
        {
            var targetActor = AnimusGameManager.EntityRegistry.FindByGameKey<AnimusActor>(targetActorKey);
            if (targetActor == null)
            {
                return UniTask.FromResult("");
            }

            // Since we interact with a certain agent that agent's context is now outdated
            ActionQueueManager.Instance?.CancelAgentRequest(targetActorKey);

            if (ConversationAnchor.ConversationAnchors.TryGetValue(_agent.gameKey, out var anchor))
            {
                AnimusAgent.SharedHistory.AddLine(new List<string>(anchor.Participants), _agent.gameKey, finalMessage);

                // Remove the agent from the anchor
                anchor.RemoveParticipant(_agent.gameKey);
            }
            else
            {
                Debug.Log("[LeaveConversation] Critical Error: Trying to leave non-existing conversation anchor. Shouldn't be possible.");
            }

            _brain.StartGoalTalk(finalMessage, targetActor);

            return UniTask.FromResult($"I said goodbye to {targetActor.gameKey} and left the conversation.");
        }
        
        [AgentAction("pickup_item", "Pickup the specified item.")]
        public UniTask<string> Pickup(string itemKey)
        {
            if (ConversationAnchor.ConversationAnchors.TryGetValue(_agent.gameKey, out var anchor))
            {
                anchor.RemoveParticipant(_agent.gameKey);
            }
            
            var item = AnimusGameManager.EntityRegistry.FindByGameKey<AnimusObject>(itemKey);
            if (item == null)
            {
                return UniTask.FromResult("");
            }

            _brain.StartGoalPickupItem(item);

            return UniTask.FromResult($"I picked up the {item.gameKey}.");
        }
    }
}