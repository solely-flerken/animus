using System;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Event
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

        public static event Action<AnimusEvent> OnDialogEvent;

        public static void InvokeDialogEvent(AnimusEvent animusEvent)
        {
            OnDialogEvent?.Invoke(animusEvent);
        }
    }
}