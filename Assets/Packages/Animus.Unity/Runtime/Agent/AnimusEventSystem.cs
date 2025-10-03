using System;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent
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

        public static event Action<string> OnLlmResponse;

        public static void InvokeLlmResponse(string message)
        {
            OnLlmResponse?.Invoke(message);
        }
    }
}