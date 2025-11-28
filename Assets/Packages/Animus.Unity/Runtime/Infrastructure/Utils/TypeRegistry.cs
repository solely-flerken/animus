using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Packages.Animus.Unity.Runtime.Infrastructure.Utils
{
    public abstract class TypeRegistry<TSelf, TItem> where TSelf : TypeRegistry<TSelf, TItem> where TItem : MonoBehaviour
    {
        public readonly List<TItem> allItems = new();

        private readonly Dictionary<Type, IList> _cache = new();

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

        public TSub GetRandom<TSub>() where TSub : TItem
        {
            var items = GetAll<TSub>();
            if (items == null || items.Count == 0)
                return null;

            var index = Random.Range(0, items.Count);
            return items[index];
        }
        
        public void Clear() 
        {
            allItems.Clear();
            _cache.Clear();
        }
    }
}