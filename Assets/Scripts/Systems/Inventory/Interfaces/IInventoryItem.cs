using System.Collections.Generic;

namespace Assets.Scripts.Systems.Inventory.Interfaces
{
    public interface IInventoryItem
    {
        public bool ValidateInsertion(IEnumerable<IInventoryItem> items);
        public bool ValidateRemoval(IEnumerable<IInventoryItem> items);
    }
}
