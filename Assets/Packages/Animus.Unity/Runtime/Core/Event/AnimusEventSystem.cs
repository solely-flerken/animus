using System;
using System.Linq;
using Packages.Animus.Unity.Runtime.Agent;
using Packages.Animus.Unity.Runtime.Core.Entity;
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
            if (animusEvent is not DialogEvent)
            {
                Debug.Log("Expected animusEvent to be DialogEvent");
                return;
            }
            
            const float maxDistance = 10f; // TODO: Specify this in some kind of setting

            // Make other agents in a certain range observe this event.
            var allAnimusAgents = AnimusEntityRegistry.Instance.GetAll<AnimusAgent>();
            foreach (var animusAgent in allAnimusAgents)
            {
                var distanceSqr = (animusAgent.transform.position - animusEvent.EventLocation).sqrMagnitude;
                if (distanceSqr < maxDistance)
                {
                    animusAgent.eventHistory.Events.Add(animusEvent);
                }
            }

            if (animusEvent.EventSource is AnimusAgent sourceAgent)
            {
                sourceAgent.eventHistory.Events.Add(animusEvent);
            }

            foreach (var agent in animusEvent.EventTarget.OfType<AnimusAgent>())
            {
                agent.eventHistory.Events.Add(animusEvent);
            }

            OnDialogEvent?.Invoke(animusEvent);
        }
    }
}