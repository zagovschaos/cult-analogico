using Assets.Scripts.Systems.Inventory;
using UnityEngine;

namespace Assets.Scripts.Systems.GameEvents.Inventory
{
    [CreateAssetMenu(fileName = "Inventory", menuName = "Scriptable Objects/Inventory/Game Event Inventory")]
    public class GameEventInventory : DefaultInventory
    {
        [Header("Events")]
        public GameEvent RefreshInventory;
        public GameEvent ItemAdded;
        public GameEvent ItemRemoved;

        public override void AddItem(InventoryItem inventoryItem)
        {
            base.AddItem(inventoryItem);
            RefreshInventory.Broadcast();
            ItemAdded.Broadcast(inventoryItem);
        }

        public override void RemoveItem(InventoryItem inventoryItem)
        {
            base.RemoveItem(inventoryItem);
            RefreshInventory.Broadcast();
            ItemRemoved.Broadcast(inventoryItem);
        }
    }
}

