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
    [RequireComponent(typeof(AnimusAgent), typeof(AgentActionSystem), typeof(SimpleAgentBrain))]
    public class BasicNpcActions : MonoBehaviour
    {
        private AgentActionSystem _actionSystem;
        private AnimusAgent _agent;
        private SimpleAgentBrain _brain;

        private void Awake()
        {
            _actionSystem = GetComponent<AgentActionSystem>();
            _agent = GetComponent<AnimusAgent>();
            _brain = GetComponent<SimpleAgentBrain>();
        }

        private void Start()
        {
            RegisterIdle();
            RegisterMoveTo();
            RegisterTalk();
            RegisterLeaveConversation();
            RegisterPickup();
        }

        private void RegisterIdle()
        {
            var idleAction = new AgentAction("idle", "Remain idle and take no action.",
                _ => UniTask.FromResult(string.Empty));

            _actionSystem.RegisterAction(idleAction);
        }

        private void RegisterMoveTo()
        {
            var moveAction = new AgentAction("move_to", "Moves to a target.",
                logic: async (args) =>
                {
                    var entityKey = args["entityKey"].ToString();
                    return await MoveTo(entityKey);
                },
                condition: () => !IsTalking()
            );
            
            moveAction.AddParam<string>("entityKey");
        
            _actionSystem.RegisterAction(moveAction);
        }

        private void RegisterTalk()
        {
            var talkAction = new AgentAction("talk", "Say something.",
                logic: async (args) =>
                {
                    var message = args["message"].ToString();
                    var targetActorKey = args["targetActorKey"].ToString();
                    return await Talk(message, targetActorKey);
                },
                condition: () =>
                {
                    if (ConversationAnchor.ConversationAnchors.TryGetValue(_agent.gameKey, out var anchor))
                    {
                        return anchor.CanContinueTalking();
                    }

                    return true;
                }
            );
            
            talkAction.AddParam<string>("message")
                .AddParam<string>("targetActorKey");
            
            _actionSystem.RegisterAction(talkAction);
        }

        private void RegisterLeaveConversation()
        {
            var leaveConversationAction = new AgentAction("leave_conversation", "End the active conversation. Use this if you want to perform a physical action or if the conversation is finished.",
                logic: async (args) =>
                {
                    var message = args["finalMessage"].ToString();
                    var targetActorKey = args["targetActorKey"].ToString();
                    return await LeaveConversation(message, targetActorKey);
                },
                condition: IsTalking
            );
            
            leaveConversationAction.AddParam<string>("finalMessage")
                .AddParam<string>("targetActorKey");
            
            _actionSystem.RegisterAction(leaveConversationAction);
        }
        
        private void RegisterPickup()
        {
            var pickupAction = new AgentAction("pickup_item", "Pickup the specified item.",
                logic: async (args) =>
                {
                    var itemKey = args["itemKey"].ToString();
                    return await Pickup(itemKey);
                },
                condition: AnyObjectVisible
            );
            
            pickupAction.AddParam<string>("itemKey");
            
            _actionSystem.RegisterAction(pickupAction);
        }
        
        private UniTask<string> MoveTo(string entityKey)
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
            _brain.StartGoalMoveTo(targetEntity.transform);

            return UniTask.FromResult($"I started moving towards {targetEntity.gameKey}.");
        }
        
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

            // Return nothing here since conversations are already saved in a conversation history.
            return UniTask.FromResult(string.Empty);
        }
        
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

            // Return nothing here since conversations are already saved in a conversation history.
            return UniTask.FromResult(string.Empty);
        }
        
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
        
        private bool IsTalking()
        {
            return ConversationAnchor.ConversationAnchors.ContainsKey(_agent.gameKey);
        }
        
        private bool AnyObjectVisible()
        {
            // TODO:
            return false;
        }
    }
}