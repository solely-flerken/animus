using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Settings
{
    [CreateAssetMenu(fileName = "AnimusSettings", menuName = "Animus/Settings")]
    public class AnimusSettings : ScriptableObject
    {
        public string apiServiceUrl = "http://127.0.0.1:8000";
        public float pollingInterval = 5.0f;

        private static AnimusSettings _instance;

        public static AnimusSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<AnimusSettings>("AnimusSettings");
                }

                if (_instance == null)
                {
                    Debug.LogError("Could not find AnimusSettings in Resources!");
                }

                return _instance;
            }
        }
    }
}