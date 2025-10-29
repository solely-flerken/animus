using System;
using Packages.Animus.Unity.Runtime.Core.Entity;
using Packages.Animus.Unity.Runtime.Core.Utils;
using Packages.Animus.Unity.Runtime.Core.Utils.TypeSelection;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    [Serializable]
    public class ActionParameter
    {
        public string name;
        [TypeFilter(typeof(AnimusEntity))] public SerializableType type;
    }
}