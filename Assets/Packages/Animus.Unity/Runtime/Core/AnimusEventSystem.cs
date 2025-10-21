using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core
{
    public class AnimusEventSystem : MonoBehaviour
    {
        public static AnimusEventSystem Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}