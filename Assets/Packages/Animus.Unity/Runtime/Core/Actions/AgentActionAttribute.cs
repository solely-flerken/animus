using System;

namespace Packages.Animus.Unity.Runtime.Core.Actions
{
    [Obsolete("Use AgentAction instead.")]
    [AttributeUsage(AttributeTargets.Method)]
    public class AgentActionAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }

        public AgentActionAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}