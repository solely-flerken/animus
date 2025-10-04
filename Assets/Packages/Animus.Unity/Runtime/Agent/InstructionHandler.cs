using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Linq;
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
            AnimusEventSystem.OnLlmResponse += ProcessInstruction;
        }

        private void OnDisable()
        {
            AnimusEventSystem.OnLlmResponse -= ProcessInstruction;
        }

        private static void ProcessInstruction(string json)
        {
            var parsedJson = JObject.Parse(json);

            var agentName = parsedJson["targetAgent"]?.Value<string>();
            if (string.IsNullOrEmpty(agentName))
            {
                Debug.LogError("Command failed: 'targetAgent' property missing or empty in JSON.");
                return;
            }

            var targetAgent = AgentRegistry.Instance?.FindByGameKey(agentName);
            if (targetAgent == null)
            {
                Debug.LogError($"Command failed: Agent '{agentName}' not found in the registry.");
                return;
            }

            if (targetAgent.instructionRegistry == null)
            {
                Debug.LogError($"Command failed: Agent '{agentName}' does not have an instruction registry assigned.");
                return;
            }

            if (parsedJson["command"]?.First is not JProperty commandProperty)
            {
                Debug.LogError("Command failed: Command property not found.");
                return;
            }

            var instructionKey = commandProperty.Name;
            var rawPayload = commandProperty.Value;

            var instruction = targetAgent.instructionRegistry.GetInstruction(instructionKey);
            if (instruction != null)
            {
                var payloadDict = rawPayload.ToObject<Dictionary<string, object>>();
                Debug.Log($"Agent '{agentName}' is executing command '{instructionKey}'.");
                instruction.Execute(targetAgent, payloadDict);
            }
            else
            {
                Debug.LogWarning(
                    $"Command ignored: Agent '{agentName}' (Profile: {targetAgent.instructionRegistry.name}) is not capable of performing the action '{instructionKey}'.");
            }
        }
    }
}