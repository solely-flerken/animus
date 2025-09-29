using Packages.Animus.Unity.Runtime.Settings;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core
{
    public class AnimusServiceManager : MonoBehaviour
    {
        [SerializeField] private AnimusSettings settings;
        
        public static AnimusServiceManager Instance { get; private set; }

        public static IAnimusService Service { get; private set; }
        
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
                return;
            }

            Service = new AnimusService(settings);
        }
    }
}