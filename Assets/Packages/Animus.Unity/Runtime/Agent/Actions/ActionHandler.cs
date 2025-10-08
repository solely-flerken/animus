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

        private void Start()
        {
            AnimusEventSystem.OnActionReceived += ProcessAction;
        }

        private void OnDisable()
        {
            AnimusEventSystem.OnActionReceived -= ProcessAction;
        }

        private static void ProcessAction(ActionPayload actionPayload)
        {
            var targetAgent = AgentRegistry.Instance?.AllItems.First(a => a.agentModel.id == actionPayload.agentId);
            if (targetAgent == null)
            {
                Debug.LogError($"Command failed: Agent '{actionPayload.agentId}' not found in the registry.");
                return;
            }

            if (targetAgent.actionRegistry == null)
            {
                Debug.LogError(
                    $"Command failed: Agent '{targetAgent.agentModel.gameKey}' does not have an action registry assigned.");
                return;
            }

            var actionKey = actionPayload.action;

            var action = targetAgent.actionRegistry.GetAction(actionKey);
            if (action != null)
            {
                Debug.Log($"Agent '{targetAgent.agentModel.gameKey}' is executing command '{actionKey}'.");
                action.Execute(targetAgent, null);
            }
            else
            {
                Debug.LogWarning(
                    $"Command ignored: Agent '{targetAgent.agentModel.gameKey}' (Profile: {targetAgent.actionRegistry.name}) is not capable of performing the action '{actionKey}'.");
            }
        }
    }
}