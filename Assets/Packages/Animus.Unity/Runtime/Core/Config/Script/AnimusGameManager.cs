using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Config.Script
{
    [DefaultExecutionOrder(-100)]
    public class AnimusGameManager : MonoBehaviour
    {
        [Header("Configuration")] 
        [SerializeField] private AnimusAgentRuntimeConfig animusConfig;

        private static AnimusGameManager _instance;

        public static AnimusGameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<AnimusGameManager>();
                }

                return _instance;
            }
        }

        public static AnimusAgentRuntimeConfig Config => Instance != null ? Instance.animusConfig : null;

        public static AnimusEntityRegistry EntityRegistry
        {
            get
            {
                if (Instance == null) return null;

                if (Instance.animusConfig == null)
                {
                    Debug.LogError("[GameManager] AnimusConfig is not assigned in the Inspector!");
                    return null;
                }

                return Instance.animusConfig.EntityRegistry;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}