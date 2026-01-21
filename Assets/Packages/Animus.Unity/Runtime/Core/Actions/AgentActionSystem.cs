using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Actions
{
    public class AgentActionSystem : MonoBehaviour
    {
        private readonly Dictionary<string, AgentAction> _actions = new();

        private string _currentActionName;
        private Dictionary<string, object> _currentActionParams;
        
        public void RegisterAction(AgentAction action)
        {
            if (_actions.ContainsKey(action.Name))
            {
                Debug.LogWarning($"[AgentActionSystem] Overwriting action: {action.Name}");
            }
            _actions[action.Name] = action;
        }

        public void UnregisterAction(string actionName)
        {
            _actions.Remove(actionName);
        }

        public List<ActionDefinition> GenerateSchema()
        {
            var schema = new List<ActionDefinition>();

            foreach (var action in _actions.Values)
            {
                if (!action.IsAvailable()) continue;

                schema.Add(new ActionDefinition
                {
                    actionKey = action.Name,
                    description = action.Description,
                    parameters = action.Parameters.Select(p => p.paramName).ToList()
                });
            }
            
            return schema;
        }

        public async UniTask<string> ExecuteAction(string actionName, Dictionary<string, object> args)
        {
            if (!_actions.TryGetValue(actionName, out var action))
                return "Error: Action not found.";

            if (!action.IsAvailable())
                return "Error: Action is currently unavailable.";

            if (IsDuplicateRequest(actionName, args))
            {
                Debug.Log($"[AgentActionSystem] Duplicate action: {actionName}");
                return null;
            }

            _currentActionName = actionName;
            _currentActionParams = args;

            try
            {
                return await action.ExecuteAsync(args);
            }
            finally
            {
                if (_currentActionName == actionName)
                {
                    _currentActionName = null;
                    _currentActionParams = null;
                }
            }
        }
        
        private bool IsDuplicateRequest(string actionName, Dictionary<string, object> actionArgs)
        {
            // If nothing is running, obviously not a duplicate
            if (string.IsNullOrEmpty(_currentActionName)) return false;

            // Action mismatch?
            if (_currentActionName != actionName) return false;

            // Arg count mismatch?
            if ((_currentActionParams?.Count ?? 0) != (actionArgs?.Count ?? 0)) return false;

            if (_currentActionParams == null || actionArgs == null) return true;
            
            foreach (var kvp in actionArgs)
            {
                if (!_currentActionParams.TryGetValue(kvp.Key, out var currentVal)) return false;

                var v1 = currentVal?.ToString() ?? "null";
                var v2 = kvp.Value?.ToString() ?? "null";

                if (v1 != v2) return false;
            }

            return true;
        }
    }
}