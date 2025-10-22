using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core
{
    /// <summary>
    /// Generic base class for creating type-safe singleton registries in Unity.
    /// </summary>
    /// <typeparam name="TSelf">
    /// The concrete registry class inheriting from <see cref="TypeRegistry{TSelf, TItem}"/>.
    /// This is used to provide a strongly-typed <see cref="Instance"/> property for the subclass.
    /// </typeparam>
    /// <typeparam name="TItem">
    /// The type of object managed by this registry (must be a <see cref="MonoBehaviour"/>).
    /// </typeparam>
    /// <remarks>
    /// This class implements a self-referential generic pattern (CRTP) to ensure that each registry
    /// has a strongly typed singleton <see cref="Instance"/> property. It maintains a global list of 
    /// items that can be registered and unregistered at runtime.
    /// </remarks>
    public abstract class TypeRegistry<TSelf, TItem> : MonoBehaviour
        where TSelf : TypeRegistry<TSelf, TItem>
        where TItem : MonoBehaviour
    {
        public static TSelf Instance { get; private set; }

        public readonly List<TItem> allItems = new();

        private readonly Dictionary<Type, IList> _cache = new();
        
        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = (TSelf)this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Register(TItem item)
        {
            if (!allItems.Contains(item))
            {
                allItems.Add(item);
                _cache.Clear();
            }
        }

        public void Unregister(TItem item)
        {
            if (allItems.Contains(item))
            {
                allItems.Remove(item);
                _cache.Clear();
            }
        }

        public List<TSub> GetAll<TSub>() where TSub : TItem
        {
            var type = typeof(TSub);
            
            if (_cache.TryGetValue(type, out var list))
            {
                return (List<TSub>)list;
            }
            
            List<TSub> result = new();
            foreach (var item in allItems)
            {
                if (item is TSub sub)
                {
                    result.Add(sub);
                }
            }

            _cache[type] = result;
            
            return result;
        }
    }
}