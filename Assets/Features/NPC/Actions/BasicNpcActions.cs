using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Features.Goap.Agents;
using Packages.Animus.Unity.Runtime.Core.Actions;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Modules.Agent;
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
            var targetEntity = AnimusEntityRegistry.Instance.FindByGameKey<AnimusEntity>(entityKey);
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
            var targetActor = AnimusEntityRegistry.Instance.FindByGameKey<AnimusActor>(targetActorKey);
            if (targetActor == null)
            {
                return UniTask.FromResult($"failure: actor '{targetActorKey}' not found");
            }

            AnimusAgent.SharedHistory.AddLine(new List<string> { _agent.gameKey, targetActor.gameKey }, _agent.gameKey, message);
            _brain.StartGoalTalk(message, targetActor);

            return UniTask.FromResult($"success: said '{message}' to {targetActor.gameKey}");
        }

        [AgentAction("pickup_item", "Pickup the specified item.")]
        public UniTask<string> Pickup(string itemKey)
        {
            var item = AnimusEntityRegistry.Instance.FindByGameKey<AnimusObject>(itemKey);
            if (item == null)
            {
                return UniTask.FromResult($"failure: item '{itemKey}' not found");
            }

            _brain.StartGoalPickupItem(item);

            return UniTask.FromResult($"success: picked up {item.gameKey}");
        }
    }
}