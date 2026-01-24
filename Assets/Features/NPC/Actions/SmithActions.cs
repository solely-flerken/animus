using System.Threading;
using Cysharp.Threading.Tasks;
using Features.Goap.Agents;
using Packages.Animus.Unity.Runtime.Core.Actions;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Modules.Conversation;
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
            _brain = GetComponent<SimpleAgentBrain>();
        }

        private void Start()
        {
            RegisterWork();
        }

        private void RegisterWork()
        {
            var workAction = new AgentAction("work", "Work at your forgery.",
                logic: async (_, token) =>
                {
                    if (workLocation == null)
                    {
                        Debug.LogError($"{nameof(workLocation)} is null");
                        return string.Empty;
                    }

                    return await MoveTo(workLocation, token);
                },
                condition: null
            );

            _actionSystem.RegisterAction(workAction);
        }

        private async UniTask<string> MoveTo(Transform targetTransform, CancellationToken token)
        {
            if (ConversationAnchor.ConversationAnchors.TryGetValue(_agent.gameKey, out var anchor))
            {
                anchor.RemoveParticipant(_agent.gameKey);
            }

            _agent.memorySystem.AddMemory("On the way to work at the forgery...");
            
            await _brain.StartGoalMoveTo(targetTransform, token);

            _agent.memorySystem.AddMemory("Currently working at the forgery.");
            
            // Runs indefinitely. Working should be stopped only when performing another action.
            await UniTask.WaitUntilCanceled(this.GetCancellationTokenOnDestroy());
            
            return "Finished working at the forgery.";
        }
    }
}