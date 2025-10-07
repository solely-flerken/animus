using System.Linq;
using Packages.Animus.Unity.Runtime.Agent.Actions;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent
{
    public class InstructionHandler : MonoBehaviour
    {
        public static InstructionHandler Instance { get; private set; }

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
            AnimusEventSystem.OnActionReceived += ProcessInstruction;
        }

        private void OnDisable()
        {
            AnimusEventSystem.OnActionReceived -= ProcessInstruction;
        }

        private static void ProcessInstruction(ActionPayload actionPayload)
        {
            var targetAgent = AgentRegistry.Instance?.AllItems.First(a => a.agentModel.id == actionPayload.agentId);
            if (targetAgent == null)
            {
                Debug.LogError($"Command failed: Agent '{actionPayload.agentId}' not found in the registry.");
                return;
            }

            if (targetAgent.instructionRegistry == null)
            {
                Debug.LogError(
                    $"Command failed: Agent '{targetAgent.agentModel.gameKey}' does not have an instruction registry assigned.");
                return;
            }

            var instructionKey = actionPayload.action;

            var instruction = targetAgent.instructionRegistry.GetInstruction(instructionKey);
            if (instruction != null)
            {
                Debug.Log($"Agent '{targetAgent.agentModel.gameKey}' is executing command '{instructionKey}'.");
                instruction.Execute(targetAgent, null);
            }
            else
            {
                Debug.LogWarning(
                    $"Command ignored: Agent '{targetAgent.agentModel.gameKey}' (Profile: {targetAgent.instructionRegistry.name}) is not capable of performing the action '{instructionKey}'.");
            }
        }
    }
}