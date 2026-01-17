using System;
using System.Collections.Generic;

namespace Packages.Animus.Unity.Runtime.Core.Actions
{
    [Serializable]
    public class ActionDefinition
    {
        public string actionKey;
        public string description;
        public List<string> parameters = new();
    }

    [Serializable] [Obsolete("Omit the parameter type since they are all string ids.")]
    public class ActionParameterDefinition
    {
        public string name;
        public string type;
    }

    [Serializable]
    public class ActionPayload
    {
        public string agentKey;
        public string reasoning;
        public string reflection;
        public string motivation;
        public string goalKey;
        public List<ActionPayloadParameter> parameters;
    }

    [Serializable]
    public class ActionPayloadParameter
    {
        public string name;
        public string value;
    }
}