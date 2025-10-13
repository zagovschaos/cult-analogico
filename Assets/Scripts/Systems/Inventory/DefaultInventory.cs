using Assets.Scripts.Systems.Inventory.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

namespace Assets.Scripts.Systems.Inventory
{
    [CreateAssetMenu(fileName = "Inventory", menuName = "Scriptable Objects/Inventory/Default Inventory")]
    public class DefaultInventory : ScriptableObject, ISerializationCallbackReceiver
    {
        public bool LoggingEnabled = false;
        public Dictionary<InventoryItem, int> Items = new();        
        public List<InventoryQuest> ActiveQuests = new();
        public List<InventoryQuest> CompletedQuests = new();

        #region Items
        public virtual void AddItem(InventoryItem inventoryItem)
        {
            if (Items.ContainsKey(inventoryItem))
            {
                Items[inventoryItem]++;

                if (LoggingEnabled)
                    Debug.Log($"Item '{inventoryItem.name}' adicionado. Quantidade atual: {Items[inventoryItem]}");

                return;
            }

            Items.Add(inventoryItem, 1);

            if (LoggingEnabled)
                Debug.Log($"Item '{inventoryItem.name}' adicionado. Quantidade atual: {Items[inventoryItem]}");
        }

        public virtual void RemoveItem(InventoryItem inventoryItem)
        {
            if (!Items.ContainsKey(inventoryItem))
            {
                if (LoggingEnabled)
                    Debug.LogWarning($"Item '{inventoryItem.name}' não pôde ser removido pois não está no inventário.");

                return;
            }

            if (Items[inventoryItem] == 1)
            {
                Items.Remove(inventoryItem);

                if (LoggingEnabled)
                    Debug.Log($"Item '{inventoryItem.name}' completamente removido do inventário.");

                return;
            }

            Items[inventoryItem]--;

            if (LoggingEnabled)
                Debug.Log($"Item '{inventoryItem.name}' removido. Quantidade atual: {Items[inventoryItem]}");
        }
        #endregion

        #region Quests
        public virtual void AddQuest(InventoryQuest quest)
        {
            if(ActiveQuests.Contains(quest))
            {
                if (LoggingEnabled)
                    Debug.LogWarning($"Quest '{quest.name}' já está ativa.");
                return;
            }

            var dependencies = new List<InventoryQuest>();
            foreach (var dependency in quest.Dependencies) 
                if(!CompletedQuests.Contains(dependency))
                    dependencies.Add(dependency);

            if (dependencies.Count > 0) 
            {
                var depNames = "";
                foreach (var dep in dependencies.Select(dep => dep.name))
                    depNames += $" - {dep} \n";

                if (LoggingEnabled)
                    Debug.LogWarning($"Quest '{quest.name}' não pode ser ativada pois possui dependências:\n{depNames}");

                return;            
            }

            ActiveQuests.Add(quest);
            if (LoggingEnabled)
                Debug.Log($"Quest '{quest.name}' ativada");
        }

        public virtual void CompleteQuest(InventoryQuest quest)
        {
            if (!ActiveQuests.Contains(quest))
            {
                if (LoggingEnabled)
                    Debug.LogWarning($"Quest '{quest.name}' não está ativa.");
                return;
            }

            foreach (var requisite in quest.Requisites.Distinct()) 
            {
                var quantity = quest.Requisites.FindAll(r => r == requisite).Count;
                if (!(Items.TryGetValue(requisite, out var invQuantity) && invQuantity >= quantity))
                {
                    if (LoggingEnabled)
                        Debug.LogWarning($"Não é possível completar a quest '{quest.name}' pois faltam items no inventário.");
                    return;
                }
            }

            foreach (var requisite in quest.Requisites.Distinct())
            {
                var quantity = quest.Requisites.FindAll(r => r == requisite).Count;
                for (var i = 0; i < quantity; i++)
                    RemoveItem(requisite);
            }

            ActiveQuests.Remove(quest);
            CompletedQuests.Add(quest);
            if (LoggingEnabled)
                Debug.Log($"Quest '{quest.name}' completada.");
        }
        #endregion

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

