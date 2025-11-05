using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    public abstract class NpcAction : ScriptableObject
    {
        public string actionKey;
        public string description;
        public List<ActionParameter> parameters = new();

        public abstract void OnExecute(AnimusAgent agent);
        
        public void Execute(AnimusAgent animusAgent, List<ActionPayloadParameter> payloadParameters)
        {
            MapParameters(payloadParameters);
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
                var payloadParam = payloadParameters.FirstOrDefault(p =>
                    p.name.Equals(paramDefinition.name, StringComparison.OrdinalIgnoreCase));

                if (payloadParam == null)
                {
                    Debug.LogWarning($"Parameter '{paramDefinition.name}' was not provided in the payload for action '{this.name}'.");
                    continue;
                }

                // Find the field in this class that matches the parameter definition's name
                var fieldToSet = fields.FirstOrDefault(f => f.Name.Equals(paramDefinition.name, StringComparison.OrdinalIgnoreCase));

                if (fieldToSet != null)
                {
                    try
                    {
                        // Convert the string value from the payload to the actual type of the field
                        var value = Convert.ChangeType(payloadParam.value, fieldToSet.FieldType);
                        fieldToSet.SetValue(this, value);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to map parameter '{paramDefinition.name}'. Could not convert '{payloadParam.value}' to type '{fieldToSet.FieldType}'. Exception: {ex.Message}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Action '{name}' has a parameter defined for '{paramDefinition.name}' but no corresponding field was found in the class '{GetType().Name}'.");
                }
            }
        }
    }
}