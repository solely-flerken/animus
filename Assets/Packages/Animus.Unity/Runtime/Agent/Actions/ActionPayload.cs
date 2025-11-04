using System;
using System.Collections.Generic;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    [Serializable]
    public class ActionPayload<T>
    {
        public string agentKey;
        public string actionKey;
        public List<ActionPayloadParameter> parameters; 
        public T details;
    }
}