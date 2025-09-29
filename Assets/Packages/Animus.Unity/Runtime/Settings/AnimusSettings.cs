using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Settings
{
    [CreateAssetMenu(fileName = "AnimusSettings", menuName = "Animus/Settings")]
    public class AnimusSettings : ScriptableObject
    {
        public string apiServiceUrl = "http://127.0.0.1:8000";
    }
}