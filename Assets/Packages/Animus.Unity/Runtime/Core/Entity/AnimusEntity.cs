using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Entity
{
    public abstract class AnimusEntity : MonoBehaviour
    {
        public string gameKey;
        public string entityName;
        public string description;
        public abstract AnimusEntityType Type { get; }
    }
}