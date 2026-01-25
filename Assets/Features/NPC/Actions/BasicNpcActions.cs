using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.Goap.Agents;
using Packages.Animus.Unity.Runtime.Core.Actions;
using Packages.Animus.Unity.Runtime.Core.Config.Script;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Modules.Conversation;
using Packages.Animus.Unity.Runtime.Modules.Environment;
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
                async (_, token) =>
                {
                    await UniTask.WaitUntilCanceled(token);
                    return null;
                });

            _actionSystem.RegisterAction(idleAction);
        }

        private void RegisterMoveTo()
        {
            var moveAction = new AgentAction("move_to", "Moves to a target.",
                logic: async (args, token) =>
                {
                    var entityKey = args["entityKey"].ToString();
                    return await MoveTo(entityKey, token);
                },
                condition: () => !IsTalking()
            );
            
            moveAction.AddParam<string>("entityKey");
        
            _actionSystem.RegisterAction(moveAction);
        }

        private void RegisterTalk()
        {
            var talkAction = new AgentAction("talk", "Say something.",
                logic: async (args, token) =>
                {
                    var message = args["message"].ToString();
                    var targetActorKey = args["entityKey"].ToString();
                    return await Talk(message, targetActorKey, token);
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
                .AddParam<string>("entityKey");
            
            _actionSystem.RegisterAction(talkAction);
        }

        private void RegisterLeaveConversation()
        {
            var leaveConversationAction = new AgentAction("leave_conversation", "End the active conversation. Use this if you want to perform a physical action or if the conversation is finished.",
                logic: async (args, token) =>
                {
                    var message = args["finalMessage"].ToString();
                    var targetActorKey = args["targetActorKey"].ToString();
                    return await LeaveConversation(message, targetActorKey, token);
                },
                condition: IsTalking,
                onSuccess: _ =>
                {
                    if (ConversationAnchor.ConversationAnchors.TryGetValue(_agent.gameKey, out var anchor))
                    {
                        // Remove the agent from the anchor
                        anchor.RemoveParticipant(_agent.gameKey);
                    }
                    else
                    {
                        Debug.Log("[LeaveConversation] Critical Error: Trying to leave non-existing conversation anchor. Shouldn't be possible.");
                    }
                }
            );
            
            leaveConversationAction.AddParam<string>("finalMessage")
                .AddParam<string>("targetActorKey");
            
            _actionSystem.RegisterAction(leaveConversationAction);
        }
        
        private void RegisterPickup()
        {
            var pickupAction = new AgentAction("pickup_item", "Pickup the specified item.",
                logic: async (args, token) =>
                {
                    var itemKey = args["itemKey"].ToString();
                    return await Pickup(itemKey, token);
                },
                condition: AnyObjectVisible
            );
            
            pickupAction.AddParam<string>("itemKey");
            
            _actionSystem.RegisterAction(pickupAction);
        }
        
        private async UniTask<string> MoveTo(string entityKey, CancellationToken token)
        {
            if (ConversationAnchor.ConversationAnchors.TryGetValue(_agent.gameKey, out var anchor))
            {
                anchor.RemoveParticipant(_agent.gameKey);
            }
            
            var targetEntity = AnimusGameManager.EntityRegistry.FindByGameKey<AnimusEntity>(entityKey);
            if (targetEntity == null)
            {
                return $"I tried to move to '{entityKey}', but I couldn't find it.";
            }

            _agent.memorySystem.AddMemory($"Moving towards {targetEntity.gameKey}...");
            
            await _brain.StartGoalMoveTo(targetEntity.transform, token);

            return $"I arrived at {targetEntity.gameKey}.";
        }

        private async UniTask<string> Talk(string message, string targetActorKey, CancellationToken token)
        {
            var targetActor = AnimusGameManager.EntityRegistry.FindByGameKey<AnimusActor>(targetActorKey);
            if (targetActor == null)
            {
                return string.Empty;
            }
            
            var anchor = ConversationAnchor.JoinOrCreate(_agent.gameKey, targetActorKey);
            
            await _brain.StartGoalTalk(message, targetActor, token);

            AnimusAgent.SharedHistory.AddLine(new List<string>(anchor.Participants), _agent.gameKey, message);
            anchor.PassTurn(targetActorKey);
            
            // Return nothing here since conversations are already saved in a conversation history.
            return string.Empty;
        }

        private async UniTask<string> LeaveConversation(string finalMessage, string targetActorKey, CancellationToken token)
        {
            var targetActor = AnimusGameManager.EntityRegistry.FindByGameKey<AnimusActor>(targetActorKey);
            if (targetActor == null)
            {
                return string.Empty;
            }

            await _brain.StartGoalTalk(finalMessage, targetActor, token);
            
            if (ConversationAnchor.ConversationAnchors.TryGetValue(_agent.gameKey, out var anchor))
            {
                AnimusAgent.SharedHistory.AddLine(new List<string>(anchor.Participants), _agent.gameKey, finalMessage);
            }
            else
            {
                Debug.LogError("[LeaveConversation] Critical Error: Trying to add to non-existing conversation anchor. Shouldn't be possible.");
            }
            
            // Return nothing here since conversations are already saved in a conversation history.
            return string.Empty;
        }

        private async UniTask<string> Pickup(string itemKey, CancellationToken token)
        {
            if (ConversationAnchor.ConversationAnchors.TryGetValue(_agent.gameKey, out var anchor))
            {
                anchor.RemoveParticipant(_agent.gameKey);
            }
            
            var item = AnimusGameManager.EntityRegistry.FindByGameKey<AnimusObject>(itemKey);
            if (item == null)
            {
                return "Item not found.";
            }

            var itemName = item.name;
            
            _agent.memorySystem.AddMemory($"In the process of picking up the item: {itemName}...");
            
            await _brain.StartGoalPickupItem(item, token);

            return $"I picked up the item: {itemName}.";
        }
        
        private bool IsTalking()
        {
            return ConversationAnchor.ConversationAnchors.ContainsKey(_agent.gameKey);
        }

        private bool AnyObjectVisible()
        {
            return EnvironmentScanner.CreateSnapshot(_agent).VisibleObjects.Count > 0;
        }
    }
}