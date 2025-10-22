using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Entity
{
    public class AnimusEntity : MonoBehaviour
    {
        public string gameKey;
        public string entityName;
        public string description;
        public virtual AnimusEntityType Type => AnimusEntityType.None;
    }
}