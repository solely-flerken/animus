using System;
using System.Collections.Generic;
using System.Linq;
using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Features.Gate.Scripts
{
    [RequireComponent(typeof(Collider))]
    public class PerimeterSensor : MonoBehaviour
    {
        public event Action<AnimusActor> OnActorEntered;
        public event Action<AnimusActor> OnActorExited;

        private readonly HashSet<AnimusActor> _actorsInZone = new();

        private void Awake()
        {
            var col = GetComponent<Collider>();
            if (!col.isTrigger)
            {
                Debug.LogWarning($"[PerimeterSensor] Collider on {gameObject.name} is not a Trigger! Setting it now.");
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var actor = other.GetComponent<AnimusActor>();
            if (actor != null)
            {
                _actorsInZone.Add(actor);
                OnActorEntered?.Invoke(actor);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var actor = other.GetComponent<AnimusActor>();
            if (actor != null && _actorsInZone.Contains(actor))
            {
                _actorsInZone.Remove(actor);
                OnActorExited?.Invoke(actor);
            }
        }

        public List<string> GetDetectedActorsNames => _actorsInZone.Select(actor => actor.gameKey).ToList();
        public bool IsZoneClear => _actorsInZone.Count == 0;
    }
}