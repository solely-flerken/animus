using Cysharp.Threading.Tasks;
using Features.Goap.Agents;
using Packages.Animus.Unity.Runtime.Core.Actions;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Modules.Memory;
using UnityEngine;

namespace Features.NPC.Actions
{
    [RequireComponent(typeof(AnimusAgent), typeof(AgentActionSystem), typeof(SimpleAgentBrain))]
    public class SmithActions : MonoBehaviour
    {
        private AgentActionSystem _actionSystem;
        private AnimusAgent _agent;
        private SimpleAgentBrain _brain;

        [SerializeField] private Transform workLocation;

        private void Awake()
        {
            _actionSystem = GetComponent<AgentActionSystem>();
            _agent = GetComponent<AnimusAgent>();
        }

        private void Start()
        {
            RegisterWork();
        }

        private void RegisterWork()
        {
            var workAction = new AgentAction("work", "Work at your forgery.",
                logic: async _ =>
                {
                    if (workLocation == null)
                    {
                        Debug.LogError($"{nameof(workLocation)} is null");
                    }

                    return await MoveTo(workLocation);
                },
                condition: null
            );

            _actionSystem.RegisterAction(workAction);
        }

        private UniTask<string> MoveTo(Transform targetTransform)
        {
            if (ConversationAnchor.ConversationAnchors.TryGetValue(_agent.gameKey, out var anchor))
            {
                anchor.RemoveParticipant(_agent.gameKey);
            }

            // TODO: Maybe callback to add to memory that the agent arrived at his target
            _brain.StartGoalMoveTo(targetTransform);

            return UniTask.FromResult("");
        }
    }
}