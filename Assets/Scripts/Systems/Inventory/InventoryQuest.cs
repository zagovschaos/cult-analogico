using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Systems.Inventory
{
    [Serializable]
    [CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Inventory/Quest")]
    public class InventoryQuest : ScriptableObject
    {
        public string Title;
        public string Description;

        public List<InventoryItem> Requisites;
        public List<InventoryQuest> Dependencies;

        [Header("Configuration")]
        [SerializeField] private DefaultInventory Inventory;

        public void AddQuest() => Inventory.AddQuest(this);
        public void CompleteQuest() => Inventory.CompleteQuest(this);
    }
}

