using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Actions
{
    public class AgentActionSystem : MonoBehaviour
    {
        private readonly Dictionary<string, AgentAction> _actions = new();

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
                    parameters = action.Parameters.Select(p => new ActionParameterDefinition { name = p.paramName, type = p.paramType }).ToList()
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

            try
            {
                return await action.ExecuteAsync(args);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                return $"Error executing {actionName}: {ex.Message}";
            }
        }
    }
}