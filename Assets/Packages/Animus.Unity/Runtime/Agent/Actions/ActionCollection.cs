using System.Collections.Generic;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    [CreateAssetMenu(fileName = "ActionCollection", menuName = "NPC/Actions/ActionCollection")]
    public class ActionCollection : ScriptableObject
    {
        public List<NpcAction> actions;

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

        public NpcAction GetRandomAction()
        {
            return actions.Count == 0 ? null : actions[Random.Range(0, actions.Count)];
        }
    }
}