using System;

namespace Packages.Animus.Unity.Runtime.Agent.Actions
{
    [Serializable]
    public class ActionPayload<T>
    {
        public string gameKey;
        public string actionKey;
        public T details;
    }
}