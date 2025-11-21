using System;
using System.Collections.Generic;

namespace Packages.Animus.Unity.Runtime.Modules.Agent.Actions
{
    [Serializable]
    public class ActionPayload
    {
        public string agentKey;
        public string actionKey;
        public List<ActionPayloadParameter> parameters; 
    }
}