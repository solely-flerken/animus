using System;
using UnityEngine;

namespace Core.Events
{
    public class EventSystem : MonoBehaviour
    {
        public static EventSystem Instance { get; private set; }

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

        public static event Action<string, string, bool> OnMessage;

        public static void InvokeMessage(string sender, string message, bool instant)
        {
            OnMessage?.Invoke(sender, message, instant);
        }
    }
}