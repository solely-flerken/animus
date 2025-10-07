using System.Collections.Generic;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    public abstract class NpcInstruction : ScriptableObject
    {
        public string instructionKey;
        public abstract void Execute(Agent agent, Dictionary<string, object> payload);
    }
}