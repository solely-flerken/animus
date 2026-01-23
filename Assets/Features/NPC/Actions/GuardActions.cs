using Cysharp.Threading.Tasks;
using Features.Gate.Scripts;
using Features.Goap.Agents;
using Packages.Animus.Unity.Runtime.Core.Actions;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Modules.Conversation;
using UnityEngine;

namespace Features.NPC.Actions
{
    [RequireComponent(typeof(AnimusAgent), typeof(AgentActionSystem), typeof(SimpleAgentBrain))]
    public class GuardActions : MonoBehaviour
    {
        private AgentActionSystem _actionSystem;
        private AnimusAgent _agent;
        private SimpleAgentBrain _brain;

        [SerializeField] private AnimusLocation guardPost;
        [SerializeField] private InteractableGate interactableGate;

        private void Awake()
        {
            _actionSystem = GetComponent<AgentActionSystem>();
            _agent = GetComponent<AnimusAgent>();
            _brain = GetComponent<SimpleAgentBrain>();
        }

        private void Start()
        {
            RegisterWork();
            RegisterOpenGate();
            RegisterCloseGate();
        }

        private void RegisterWork()
        {
            var workAction = new AgentAction("work", "Stand guard at your post",
                logic: async _ =>
                {
                    if (guardPost == null)
                    {
                        Debug.LogError($"{nameof(guardPost)} is null");
                        return string.Empty;
                    }

                    return await StandGuard(guardPost);
                },
                condition: null
            );

            _actionSystem.RegisterAction(workAction);
        }
        
        private void RegisterOpenGate()
        {
            var openGateAction = new AgentAction("open_gate", "Open the gate.",
                logic: _ =>
                {
                    if (interactableGate == null)
                    {
                        Debug.LogError("Gate object is not assigned!");
                        return UniTask.FromResult(string.Empty);
                    }

                    interactableGate?.OpenGate();
                    return UniTask.FromResult("I opened the gate.");
                },
                condition: () => !interactableGate.IsOpen
            );

            _actionSystem.RegisterAction(openGateAction);
        }

        private void RegisterCloseGate()
        {
            var openGateAction = new AgentAction("close_gate", "Close the Gate.",
                logic: _ =>
                {
                    if (interactableGate == null)
                    {
                        Debug.LogError("Gate object is not assigned!");
                        return UniTask.FromResult(string.Empty);
                    }

                    interactableGate?.CloseGate();
                    return UniTask.FromResult("I closed the gate.");
                },
                condition: () => interactableGate.IsOpen
            );

            _actionSystem.RegisterAction(openGateAction);
        }
        
        private async UniTask<string> StandGuard(AnimusLocation location)
        {
            if (ConversationAnchor.ConversationAnchors.TryGetValue(_agent.gameKey, out var anchor))
            {
                anchor.RemoveParticipant(_agent.gameKey);
            }

            _agent.memories.Add("On the way to the guard post...");
            
            await _brain.StartGoalMoveTo(location.transform);

            _agent.memories.Add("Currently standing guard.");
            
            // Runs indefinitely. Working should be stopped only when performing another action.
            await UniTask.WaitUntilCanceled(this.GetCancellationTokenOnDestroy());
            
            return "Finished standing guard.";
        }
    }
}