using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Modules.Inventory
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Animus/Item")]
    public class ItemDefinition : ScriptableObject
    {
        [Header("Identity")] 
        public string itemTypeId;
        public string itemName;
        [TextArea] public string description;

        [Header("Economy & Physics")] 
        public int baseCost = 5;
        public float weight = 1.0f;
        public GameObject worldPrefab;
    }
}