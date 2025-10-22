using System.Linq;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    public class ActionHandler : MonoBehaviour
    {
        public static ActionHandler Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static void ProcessAction<T>(ActionPayload<T> actionPayload)
        {
            var targetAgent =
                AgentRegistry.Instance.allItems.FirstOrDefault(a => a.animusAgent.gameKey == actionPayload.gameKey);
            if (targetAgent == null)
            {
                Debug.LogError($"Command failed: Agent '{actionPayload.gameKey}' not found in the registry.");
                return;
            }

            if (targetAgent.actionCollection == null)
            {
                Debug.LogError(
                    $"Command failed: Agent '{targetAgent.animusAgent.gameKey}' does not have an action registry assigned.");
                return;
            }

            var actionKey = actionPayload.actionKey;

            var action = targetAgent.actionCollection.GetAction(actionKey);
            if (action == null)
            {
                Debug.LogWarning(
                    $"Command ignored: Agent '{targetAgent.animusAgent.gameKey}' (Profile: {targetAgent.actionCollection.name}) is not capable of performing the action '{actionKey}'.");
            }

            Debug.Log($"Agent '{targetAgent.animusAgent.gameKey}' is executing command '{actionKey}'.");
            action.Execute(targetAgent, null);
        }
    }
}