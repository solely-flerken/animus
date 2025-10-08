using System.Collections.Generic;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    [CreateAssetMenu(fileName = "ActionRegistry", menuName = "NPC/Actions/ActionRegistry")]
    public class ActionRegistry : ScriptableObject
    {
        [SerializeField] private List<NpcAction> actions;

        private Dictionary<string, NpcAction> _actionsMap;

        public void Initialize()
        {
            _actionsMap = new Dictionary<string, NpcAction>();

            foreach (var action in actions)
            {
                if (action == null || string.IsNullOrEmpty(action.actionKey))
                {
                    continue;
                }

                if (!_actionsMap.TryAdd(action.actionKey, action))
                {
                    Debug.LogWarning($"Duplicate action key found in registry: '{action.actionKey}'");
                }
            }
        }

        public NpcAction GetAction(string key)
        {
            _actionsMap.TryGetValue(key, out var action);
            return action;
        }

        public List<string> GetActionKeys()
        {
            return new List<string>(_actionsMap.Keys);
        }
    }
}