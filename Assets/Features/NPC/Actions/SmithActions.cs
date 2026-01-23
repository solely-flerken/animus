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
                logic: async _ =>
                {
                    if (workLocation == null)
                    {
                        Debug.LogError($"{nameof(workLocation)} is null");
                        return string.Empty;
                    }

                    return await MoveTo(workLocation);
                },
                condition: null
            );

            _actionSystem.RegisterAction(workAction);
        }

        private async UniTask<string> MoveTo(Transform targetTransform)
        {
            if (ConversationAnchor.ConversationAnchors.TryGetValue(_agent.gameKey, out var anchor))
            {
                anchor.RemoveParticipant(_agent.gameKey);
            }

            _agent.memories.Add("On the way to work at the forgery...");
            
            await _brain.StartGoalMoveTo(targetTransform);

            _agent.memories.Add("Currently working at the forgery.");
            
            // Runs indefinitely. Working should be stopped only when performing another action.
            await UniTask.WaitUntilCanceled(this.GetCancellationTokenOnDestroy());
            
            return "Finished working at the forgery.";
        }
    }
}