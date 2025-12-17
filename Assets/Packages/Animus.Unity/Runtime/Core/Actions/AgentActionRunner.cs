using System;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Actions
{
    [Obsolete("Use AgentActionSystem instead.")]
    public class AgentActionRunner : MonoBehaviour
    {
        private readonly Dictionary<string, MethodInfo> _registeredActions = new();
        private readonly Dictionary<string, object> _componentCache = new();

        private void Awake()
        {
            RegisterActions();
        }

        private void RegisterActions()
        {
            // Scan all MonoBehaviours on this GameObject
            var components = GetComponents<MonoBehaviour>();

            foreach (var component in components)
            {
                var methods = component.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (var method in methods)
                {
                    var attr = method.GetCustomAttribute<AgentActionAttribute>();
                    if (attr == null)
                    {
                        // Component has no Action Definition for an Agent
                        continue;
                    }
                    
                    if (!_registeredActions.TryAdd(attr.Name, method))
                    {
                        Debug.LogWarning($"Duplicate Action Key: {attr.Name}");
                        continue;
                    }

                    _componentCache.Add(attr.Name, component);
                }
            }
        }

        public async UniTask<string> Execute(string actionName, Dictionary<string, object> parameters)
        {
            if (!_registeredActions.TryGetValue(actionName, out var method))
            {
                return $"Error: Action '{actionName}' not found.";
            }

            var component = _componentCache[actionName];
            var methodParams = method.GetParameters();
            var invokeArgs = new object[methodParams.Length];

            try
            {
                for (var i = 0; i < methodParams.Length; i++)
                {
                    var paramInfo = methodParams[i];

                    if (parameters.TryGetValue(paramInfo.Name, out var value))
                    {
                        try
                        {
                            invokeArgs[i] = Convert.ChangeType(value, paramInfo.ParameterType);
                        }
                        catch
                        {
                            return $"Error: Parameter '{paramInfo.Name}' expects {paramInfo.ParameterType.Name}, got {value}";
                        }
                    }
                    else if (paramInfo.HasDefaultValue)
                    {
                        invokeArgs[i] = paramInfo.DefaultValue;
                    }
                    else
                    {
                        return $"Error: Missing required parameter '{paramInfo.Name}'";
                    }
                }

                var result = method.Invoke(component, invokeArgs);

                // Handling the result based on method signature
                if (method.ReturnType == typeof(UniTask<string>))
                {
                    return await (UniTask<string>)result;
                }

                if (method.ReturnType == typeof(UniTask))
                {
                    await (UniTask)result;
                    return "Success";
                }

                if (method.ReturnType == typeof(string))
                {
                    return (string)result;
                }

                if (method.ReturnType == typeof(void))
                {
                    return "Success";
                }

                // Fallback for unexpected types
                return result?.ToString() ?? "Executed";
            }
            catch (Exception e)
            {
                Debug.LogError($"Action Execution Critical Failure: {e}");
                return $"Critical Failure: {e.Message}";
            }
        }
        
        public List<ActionDefinition> GenerateActionSchema()
        {
            var schemaList = new List<ActionDefinition>();

            foreach (var kvp in _registeredActions)
            {
                var methodInfo = kvp.Value;
                var attr = methodInfo.GetCustomAttribute<AgentActionAttribute>();
                var parameters = methodInfo.GetParameters();

                // Build the Parameter List
                var paramDefs = new List<ActionParameterDefinition>();
                foreach (var p in parameters)
                {
                    paramDefs.Add(new ActionParameterDefinition
                    {
                        name = p.Name,
                        type = p.ParameterType.Name 
                    });
                }

                // Add to schema
                schemaList.Add(new ActionDefinition
                {
                    actionKey = attr.Name,
                    description = attr.Description,
                    parameters = paramDefs
                });
            }

            return schemaList;
        }
    }
}