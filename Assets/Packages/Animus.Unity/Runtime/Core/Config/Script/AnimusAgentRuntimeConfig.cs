using System;
using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Config.Script
{
    [CreateAssetMenu(menuName = "Animus/RuntimeSet")]
    public class AnimusAgentRuntimeConfig : ScriptableObject
    {
        [NonSerialized] private AnimusEntityRegistry _registry;
        public AnimusEntityRegistry EntityRegistry => _registry ??= new AnimusEntityRegistry();

        private void OnEnable()
        {
            _registry = new AnimusEntityRegistry();
        }

        private void OnDisable()
        {
            _registry?.Clear();
        }
    }
}