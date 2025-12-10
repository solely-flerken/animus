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

        [AgentAction("move_to", "Moves the agent to a specific entity.")]
        public UniTask<string> MoveTo(string entityKey)
        {
            var targetEntity = AnimusGameManager.EntityRegistry.FindByGameKey<AnimusEntity>(entityKey);
            if (targetEntity == null)
            {
                return UniTask.FromResult($"failure: entity '{entityKey}' not found");
            }

            _brain.StartGoalMoveTo(targetEntity);

            return UniTask.FromResult($"success: moving to {targetEntity.gameKey}");
        }

        [AgentAction("talk", "Say something to another character.")]
        public UniTask<string> Talk(string message, string targetActorKey)
        {
            var targetActor = AnimusGameManager.EntityRegistry.FindByGameKey<AnimusActor>(targetActorKey);
            if (targetActor == null)
            {
                return UniTask.FromResult($"failure: actor '{targetActorKey}' not found");
            }
            
            // Since we interact with a certain agent that agent's context is now outdated
            ActionQueueManager.Instance?.CancelAgentRequest(targetActorKey);
            
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

            return UniTask.FromResult($"success: said '{message}' to {targetActor.gameKey}");
        }

        [AgentAction("pickup_item", "Pickup the specified item.")]
        public UniTask<string> Pickup(string itemKey)
        {
            var item = AnimusGameManager.EntityRegistry.FindByGameKey<AnimusObject>(itemKey);
            if (item == null)
            {
                return UniTask.FromResult($"failure: item '{itemKey}' not found");
            }

            _brain.StartGoalPickupItem(item);

            return UniTask.FromResult($"success: picked up {item.gameKey}");
        }
    }
}