using Packages.Animus.Unity.Runtime.Core.Config.Script;
using Packages.Animus.Unity.Runtime.Modules.Inventory;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Core.Entity
{
    public class AnimusObject : AnimusEntity
    {
        public override AnimusEntityType Type => AnimusEntityType.Object;

        [Header("Data Link")] 
        public ItemDefinition itemData;
        public int quantity = 1;

        private void Start()
        {
            if (itemData != null && itemData.worldPrefab != null)
            {
                // Clear placeholder graphics
                foreach (Transform child in transform)
                {
                    Destroy(child.gameObject);
                }

                var go = Instantiate(itemData.worldPrefab, transform);
                go.transform.localPosition = Vector3.zero;
            }

            gameKey = $"{itemData.itemTypeId}_{AnimusGameManager.EntityRegistry.GetAll<AnimusObject>().Count + 1}";
            entityName = itemData.name;
            description = itemData.description;
        }

        private void OnEnable()
        {
            AnimusGameManager.EntityRegistry?.Register(this);
        }

        private void OnDisable()
        {
            AnimusGameManager.EntityRegistry?.Unregister(this);
        }

        public ItemDefinition Pickup()
        {
            Destroy(gameObject);
            return itemData;
        }
    }
}