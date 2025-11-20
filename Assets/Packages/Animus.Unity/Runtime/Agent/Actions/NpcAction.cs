using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Core.Utils.Json;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    [JsonConverter(typeof(NpcActionConverter))]
    public abstract class NpcAction : ScriptableObject
    {
        public string actionKey;
        public string description;
        public List<ActionParameter> parameters = new();

        public abstract void OnExecute(AnimusAgent agent);

        public void Execute(AnimusAgent animusAgent, ActionPayload payload)
        {
            MapParameters(payload.parameters);

            // TODO:
            var entry = ActionHistoryEntry.CreateFromPayload(payload, "", "success");
            animusAgent.actionHistory.AddEntry(entry); 
            
            OnExecute(animusAgent);
        }

        private void MapParameters(List<ActionPayloadParameter> payloadParameters)
        {
            // Get all public and non-public instance fields of the concrete class (e.g., DisplayMessageAction)
            var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            // Iterate through the parameter definitions set in the ScriptableObject
            foreach (var paramDefinition in parameters)
            {
                // Find the payload parameter sent from the LLM
                var payloadParam = payloadParameters.FirstOrDefault(p => p.name.Equals(paramDefinition.name, StringComparison.OrdinalIgnoreCase));

                if (payloadParam == null)
                {
                    Debug.LogWarning($"Parameter '{paramDefinition.name}' was not provided in the payload for action '{this.name}'.");
                    continue;
                }

                // Find the field in this class that matches the parameter definition's name
                var fieldToSet = fields.FirstOrDefault(f => f.Name.Equals(paramDefinition.name, StringComparison.OrdinalIgnoreCase));

                if (fieldToSet != null)
                {
                    object valueToSet = null;

                    // Check if the field is an AnimusEntity or a derived type (like AnimusAgent).
                    if (typeof(AnimusEntity).IsAssignableFrom(fieldToSet.FieldType))
                    {
                        if (AnimusEntityRegistry.Instance != null)
                        {
                            valueToSet = AnimusEntityRegistry.Instance.FindByGameKey(payloadParam.value, fieldToSet.FieldType);
                            if (valueToSet == null)
                            {
                                Debug.LogError($"Could not find an AnimusEntity of type '{fieldToSet.FieldType.Name}' with key '{payloadParam.value}'.");
                            }
                        }
                        else
                        {
                            Debug.LogError("AnimusEntityRegistry instance not found in the scene.");
                        }
                    }
                    else // For all other types (string, int, bool, etc.).
                    {
                        try
                        {
                            // Convert the string value from the payload to the actual type of the field
                            valueToSet = Convert.ChangeType(payloadParam.value, fieldToSet.FieldType);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Failed to convert parameter '{paramDefinition.name}' from '{payloadParam.value}' to type '{fieldToSet.FieldType.Name}'. Exception: {ex.Message}");
                            continue;
                        }
                    }

                    fieldToSet.SetValue(this, valueToSet);
                }
                else
                {
                    Debug.LogWarning($"Action '{name}' has a parameter defined for '{paramDefinition.name}' but no corresponding field was found in the class '{GetType().Name}'.");
                }
            }
        }
    }
}