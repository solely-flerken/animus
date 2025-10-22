using System;

namespace Packages.Animus.Unity.Runtime.Core
{
    [Serializable]
    public class AnimusEntity
    {
        public string gameKey;
        public string name;
        public string description;
        public virtual AnimusEntityType Type => AnimusEntityType.None;
    }
}