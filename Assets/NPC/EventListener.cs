using Events;
using UnityEngine;

namespace NPC
{
    public class EventListener : MonoBehaviour
    {
        private void OnEnable()
        {
            EventSystem.OnChatMessage += HandleCommand;
        }

        private void OnDisable()
        {
            EventSystem.OnChatMessage -= HandleCommand;
        }

        private static void HandleCommand(string command)
        {
            Debug.Log(command);
        }
    }
}