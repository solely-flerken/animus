using System.Collections.Generic;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    public abstract class NpcAction : ScriptableObject
    {
        public string actionKey;
        public abstract void Execute(Agent agent, Dictionary<string, object> payload);
    }
}