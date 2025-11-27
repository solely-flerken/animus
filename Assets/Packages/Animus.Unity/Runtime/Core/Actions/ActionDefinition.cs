using System;
using System.Collections.Generic;

namespace Packages.Animus.Unity.Runtime.Core.Actions
{
    [Serializable]
    public class ActionDefinition
    {
        public string actionKey;
        public string description;
        public List<ActionParameterDefinition> parameters = new();
    }

    [Serializable]
    public class ActionParameterDefinition
    {
        public string name;
        public string type;
    }

    [Serializable]
    public class ActionPayload
    {
        public string agentKey;
        public string actionKey;
        public List<ActionPayloadParameter> parameters;
    }

    [Serializable]
    public class ActionPayloadParameter
    {
        public string name;
        public string value;
    }
}