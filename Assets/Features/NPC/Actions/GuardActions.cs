using Cysharp.Threading.Tasks;
using Features.Gate.Scripts;
using Packages.Animus.Unity.Runtime.Core.Actions;
using UnityEngine;

namespace Features.NPC.Actions
{
    public class GuardActions : MonoBehaviour
    {
        [SerializeField] private InteractableGate interactableGate;
        
        [AgentAction("open_gate", "Open the Gate.")]
        public UniTask<string> OpenGate()
        {
            if (interactableGate == null)
            {
                Debug.LogError("Gate object is not assigned!");
                return UniTask.FromResult("");
            }
            
            interactableGate?.OpenGate();
            return UniTask.FromResult("I opened the gate.");
        }

        [AgentAction("close_gate", "Close the Gate.")]
        public UniTask<string> CloseGate()
        {
            if (interactableGate == null)
            {
                Debug.LogError("Gate object is not assigned!");
                return UniTask.FromResult("");
            }
            
            interactableGate?.CloseGate();
            return UniTask.FromResult("I closed the gate.");
        }
    }
}