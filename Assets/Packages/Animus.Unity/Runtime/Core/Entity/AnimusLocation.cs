using System.Collections.Generic;
using Packages.Animus.Unity.Runtime.Core.Config.Script;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Entity
{
    public class AnimusLocation : AnimusEntity
    {
        public override AnimusEntityType Type => AnimusEntityType.Location;

        public ContextScope contextScope = ContextScope.Global;
        
        [SerializeField] [Tooltip("Agents who can see this location in their context")]
        public List<AnimusAgent> relevantAgents = new();
        
        private void OnEnable()
        {
            AnimusGameManager.EntityRegistry?.Register(this);
        }

        private void OnDisable()
        {
            AnimusGameManager.EntityRegistry?.Unregister(this);
        }
        
        public bool IsRelevantTo(AnimusAgent agent)
        {
            if (contextScope == ContextScope.Global) return true;
            return relevantAgents?.Contains(agent) ?? false;
        }
    }
}