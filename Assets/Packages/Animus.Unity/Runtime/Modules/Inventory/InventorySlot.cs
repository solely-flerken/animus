using System;

namespace Packages.Animus.Unity.Runtime.Modules.Inventory
{
    [Serializable]
    public class InventorySlot
    {
        public ItemDefinition itemData;
        public int quantity;

        public InventorySlot(ItemDefinition item, int qty)
        {
            itemData = item;
            quantity = qty;
        }
    }
}