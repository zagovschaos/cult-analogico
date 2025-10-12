using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems.Inventory
{
    [CreateAssetMenu(fileName = "Inventory", menuName = "Scriptable Objects/Inventory/Default Inventory")]
    public class DefaultInventory : ScriptableObject, ISerializationCallbackReceiver
    {
        public Dictionary<InventoryItem, int> Items = new();        

        public virtual void AddItem(InventoryItem inventoryItem)
        {
            if (Items.ContainsKey(inventoryItem))
            {
                Items[inventoryItem]++;
                return;
            }

            Items.Add(inventoryItem, 1);
        }

        public virtual void RemoveItem(InventoryItem inventoryItem)
        {
            if (!Items.ContainsKey(inventoryItem))
                return;

            if (Items[inventoryItem] == 1)
            {
                Items.Remove(inventoryItem);
                return;
            }

            Items[inventoryItem]--;
        }

        #region Serialization
        [Header("Inventory Items")]
        [SerializeField] private List<InventoryItem> Item = new();
        [SerializeField] private List<int> Quantity = new();

        public void OnBeforeSerialize()
        {
            Item.Clear();
            Quantity.Clear();
            foreach (var pair in Items)
            {
                Item.Add(pair.Key);
                Quantity.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Items.Clear();
            for (int i = 0; i < Item.Count; i++)
            {
                if (i < Quantity.Count) // Ensure value exists for key
                {
                    Items[Item[i]] = Quantity[i];
                }
            }
        }
        #endregion
    }
}

