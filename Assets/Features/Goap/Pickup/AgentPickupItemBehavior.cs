using Packages.Animus.Unity.Runtime.Core.Entity;
using UnityEngine;

namespace Features.Goap.Pickup
{
    public class AgentPickupItemBehavior : MonoBehaviour
    {
        public AnimusObject targetItem;
        public string targetItemTypeId; 
        public int totalItemQuantityAfterPickup;
    }
}