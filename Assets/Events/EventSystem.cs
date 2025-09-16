using System;
using UnityEngine;

namespace Events
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

        public static event Action<string> OnChatMessage;

        public static void InvokeChatMessage(string message)
        {
            OnChatMessage?.Invoke(message);
        }
    }
}