using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Modules.Inventory
{
    [Serializable]
    public class Inventory
    {
        [Header("State")] 
        public int gold = 50;
        public float maxWeight = 20f;

        [SerializeField] private List<InventorySlot> slots = new();

        public bool AddItem(ItemDefinition item, int quantity = 1)
        {
            if (GetCurrentWeight() + (item.weight * quantity) > maxWeight)
            {
                // Object to heavy
                return false;
            }

            var existing = slots.Find(s => s.itemData == item);
            if (existing != null)
            {
                existing.quantity += quantity;
            }
            else
            {
                slots.Add(new InventorySlot(item, quantity));
            }

            return true;
        }

        public bool RemoveItem(string itemId, int quantity = 1)
        {
            var existing = slots.Find(s => s.itemData.itemTypeId == itemId);
            if (existing == null || existing.quantity < quantity)
            {
                return false;
            }

            existing.quantity -= quantity;
            if (existing.quantity <= 0)
            {
                slots.Remove(existing);
            }
            
            return true;
        }

        public bool HasItem(string itemId)
        {
            return slots.Exists(s => s.itemData.itemTypeId == itemId);
        }
        
        public int GetItemQuantity(string itemId)
        {
            var slot = slots.Find(s => s.itemData.itemTypeId == itemId);
            return slot?.quantity ?? 0;
        }
        
        private float GetCurrentWeight()
        {
            return slots.Sum(s => s.itemData.weight * s.quantity);
        }
    }
}