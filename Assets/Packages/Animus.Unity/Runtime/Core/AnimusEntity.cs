using System;

namespace Packages.Animus.Unity.Runtime.Core
{
    [Serializable]
    public class AnimusEntity<T>
    {
        public string gameKey;
        public string name;
        public string description;
        public AnimusEntityType type;
        public T details;
    }
}