using Cysharp.Threading.Tasks;
using Features.Gate.Scripts;
using Packages.Animus.Unity.Runtime.Core.Actions;
using UnityEngine;

namespace Features.NPC.Actions
{
    [RequireComponent(typeof(AgentActionSystem))]
    public class GuardActions : MonoBehaviour
    {
        private AgentActionSystem _actionSystem;

        [SerializeField] private InteractableGate interactableGate;

        private void Awake()
        {
            _actionSystem = GetComponent<AgentActionSystem>();
        }

        private void Start()
        {
            RegisterOpenGate();
            RegisterCloseGate();
        }

        private void RegisterOpenGate()
        {
            var openGateAction = new AgentAction("open_gate", "Open the gate.",
                logic: _ =>
                {
                    if (interactableGate == null)
                    {
                        Debug.LogError("Gate object is not assigned!");
                        return UniTask.FromResult("");
                    }

                    interactableGate?.OpenGate();
                    return UniTask.FromResult("I opened the gate.");
                },
                condition: null // TODO: Check if closed
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
                        return UniTask.FromResult("");
                    }

                    interactableGate?.CloseGate();
                    return UniTask.FromResult("I closed the gate.");
                },
                condition: null // TODO: Check if open
            );

            _actionSystem.RegisterAction(openGateAction);
        }
    }
}