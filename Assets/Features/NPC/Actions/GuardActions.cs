using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.Gate.Scripts;
using Features.Goap.Agents;
using Packages.Animus.Unity.Runtime.Core.Actions;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Integrations.Prompting;
using Packages.Animus.Unity.Runtime.Modules.Conversation;
using UnityEngine;

namespace Features.NPC.Actions
{
    [RequireComponent(typeof(AnimusAgent), typeof(AgentActionSystem), typeof(SimpleAgentBrain))]
    public class GuardActions : MonoBehaviour, ISituationalContextProvider
    {
        private AgentActionSystem _actionSystem;
        private AnimusAgent _agent;
        private SimpleAgentBrain _brain;

        [SerializeField] private AnimusLocation guardPost;
        [SerializeField] private InteractableGate interactableGate;
        [SerializeField] private PerimeterSensor perimeterSensor;

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

        private void OnEnable()
        {
            if (perimeterSensor != null)
            {
                perimeterSensor.OnActorEntered += HandlePerimeterBreach;
            }
        }

        private void OnDisable()
        {
            if (perimeterSensor != null)
            {
                perimeterSensor.OnActorEntered -= HandlePerimeterBreach;
            }
        }
        
        private void HandlePerimeterBreach(AnimusActor intruder)
        {
            if (intruder.gameKey == _agent.gameKey) return;
            
            ActionQueueManager.Instance.ForceAgentThink(_agent.gameKey, $"{intruder.gameKey} just entered the gate perimeter");
        }

        public List<string> GetSituationalContext()
        {
            var context = new List<string>();

            if (interactableGate)
            {
                var gateState = interactableGate.IsOpen ? "open" : "closed";
                context.Add($"The city gate is {gateState}.");
            }

            if (perimeterSensor)
            {
                var intruders = perimeterSensor.GetDetectedActorsNames;
                if (intruders.Count > 0)
                {
                    var names = string.Join(", ", intruders);
                    context.Add($"ATTENTION: The following individuals are currently inside the gate perimeter: {names}.");
                }
                else
                {
                    context.Add("The gate perimeter is currently clear.");
                }
            }

            return context;
        }
        
        private void RegisterWork()
        {
            var workAction = new AgentAction("guard", "Stand guard at your post",
                logic: async (_, token) =>
                {
                    if (guardPost == null)
                    {
                        Debug.LogError($"{nameof(guardPost)} is null");
                        return string.Empty;
                    }

                    return await StandGuard(guardPost, token);
                },
                condition: null
            );

            _actionSystem.RegisterAction(workAction);
        }
        
        private void RegisterOpenGate()
        {
            var openGateAction = new AgentAction("open_gate", "Open the gate.",
                logic: (_, _) =>
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
                logic: (_, _) =>
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
        
        private async UniTask<string> StandGuard(AnimusLocation location, CancellationToken token)
        {
            if (ConversationAnchor.ConversationAnchors.TryGetValue(_agent.gameKey, out var anchor))
            {
                anchor.RemoveParticipant(_agent.gameKey);
            }

            _agent.memorySystem.AddMemory("On the way to the guard post...");
            
            await _brain.StartGoalMoveTo(location.transform, token);

            _agent.memorySystem.AddMemory("Currently standing guard.");
            
            // Runs indefinitely. Working should be stopped only when performing another action.
            await UniTask.WaitUntilCanceled(token);
            
            return "Finished standing guard.";
        }
    }
}