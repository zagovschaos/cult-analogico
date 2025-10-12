using System;
using UnityEngine;

namespace Assets.Scripts.Systems.Inventory
{
    [Serializable]
    [CreateAssetMenu(fileName = "Inventory Item", menuName = "Scriptable Objects/Inventory/Inventory Item")]
    public class InventoryItem : ScriptableObject
    {
        [SerializeField] private string Description;
        [SerializeField] private Sprite Icon;

        [Header("Configuration")]
        [SerializeField] private DefaultInventory Inventory;

        public void AddToInventory() => Inventory.AddItem(this);
        public void RemoveFromInventory() => Inventory.RemoveItem(this);
    }
}

