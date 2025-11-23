using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Modules.Goap.Pickup
{
    public class AgentPickupItemBehavior : MonoBehaviour
    {
        public AnimusObject targetItem;
        public string targetItemTypeId; 
        public int totalItemQuantityAfterPickup;
    }
}