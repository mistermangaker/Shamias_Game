using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using static UnityEditor.Timeline.Actions.MenuPriority;
using System;

namespace GameSystems.Inventory
{
    [System.Serializable]
    public class InventorySystem
    {
        [SerializeField] private List<InventorySlot> _inventorySlots;
        public List<InventorySlot> InventorySlots { get {  return _inventorySlots; } }
        public int InventorySize => _inventorySlots.Count;

        public UnityAction<InventorySlot> OnInventorySlotChanged;
        
        
        public InventorySystem(int size) 
        { 
            _inventorySlots = new List<InventorySlot>(size);
            for (int i = 0; i < size; i++)
            {
                _inventorySlots.Add(new InventorySlot());
            }
        }

        public Dictionary<GameItem, int> GetAllHeldInventoryItems()
        {
            List<InventorySlot> items = _inventorySlots.Where(i => !i.IsEmpty).ToList();
            Dictionary<GameItem, int> keyValuePairs = new Dictionary<GameItem, int>();
            foreach (InventorySlot slot in items)
            {
                if (keyValuePairs.ContainsKey(slot.GameItem))
                {
                    keyValuePairs[slot.GameItem] += slot.StackSize;
                    continue;
                }
                keyValuePairs.Add(slot.GameItem, slot.StackSize);
            }
            return keyValuePairs;
        }

        public void SetSlot(GameItem item, int amount, int position)
        {
            _inventorySlots[position].AddItem(item, amount);
        }

        public void RemoveItemsFromInventory(GameItem item, int amount)
        {
            if (ContainsItem(item, out List<InventorySlot> invSlot)) // check if item exists in inventory
            {
                foreach (InventorySlot slot in invSlot)
                {
                    int removeAmount = Mathf.Min(slot.StackSize, amount);
                    slot.RemoveFromStack(removeAmount);
                    OnInventorySlotChanged?.Invoke(slot);
                    amount -= removeAmount;
                    if(amount <= 0) return;
                }
            }
        }


        public void RemoveFromInventory(int amount, int position)
        {
            InventorySlot slot = _inventorySlots[position];
            slot.RemoveFromStack(amount);
            if(slot.StackSize <= 0)
            {
                slot.ClearSlot();
                
            }
            OnInventorySlotChanged?.Invoke(slot);
        }

        public void RemoveFromInventory(InventorySlot slot, int amount)
        {
            
            int i = 0;
            foreach (InventorySlot slot2 in _inventorySlots)
            {
                if(slot2 == slot)
                {
                  
                    RemoveFromInventory(amount, i);
                }
                i++;
            }
        }
        
        
        public bool AddToInventory(GameItem item, int amount)
        {
            return AddToInventory(item, amount, out int remainder);
        }

        public bool AddToInventory(GameItem item, int amount, out int remainder)
        {
            //return AddToInventory(item.ItemType, amount, out remainder);
            remainder = 0;
            int itemMaxStackSize = item.ItemData.MaxStackSize;
            if (ContainsItem(item.ItemData, out List<InventorySlot> invSlot) && itemMaxStackSize>1) // check if item exists in inventory
            {
                foreach (InventorySlot slot in invSlot)
                {
                    int amountToAdd = slot.GetAmountToAdd(amount);
                    
                    if (amountToAdd >= 1)
                    {
                        slot.AddToStack(amountToAdd);
                        amount -= amountToAdd;
                        OnInventorySlotChanged?.Invoke(slot);
                        if (amount >= 1) continue;
                       
                        return true;
                    }
                    
                }
            }
           
            if (HasFreeSlot(out List<InventorySlot> freeSlots)) //gets the first available slot
            {
                foreach (InventorySlot freeSlot in freeSlots)
                {
                    int amountToadd = Mathf.Min(itemMaxStackSize, amount);
                    freeSlot.AddItem(item, amountToadd);
                    OnInventorySlotChanged?.Invoke(freeSlot);
                    amount -= amountToadd;
                    if (amount >= 1) continue;
                    return true;
                }
            }
            remainder = amount;
            return false;
        }

        public bool ContainsItem(GameItem itemToAdd, out List<InventorySlot> inventorySlot)
        {
            inventorySlot = InventorySlots.Where(i => i.GameItem.ItemData == itemToAdd.ItemData).ToList();
            return inventorySlot.Count >= 1 ? true : false;
        }

        public bool ContainsItem(ItemData itemToAdd, out List<InventorySlot> inventorySlot)
        {
            inventorySlot = InventorySlots.Where(i => i.ItemData == itemToAdd).ToList();
            return inventorySlot.Count >= 1 ? true : false;
        }

        public bool HasFreeSlot(out List<InventorySlot> freeSlot)
        {
            freeSlot = InventorySlots.Where(i => i.ItemData == null).ToList();
            return freeSlot == null ? false : true;
        }

        public bool HasFreeSlot(out InventorySlot freeSlot)
        {
            freeSlot = InventorySlots.FirstOrDefault(i => i.GameItem.ItemData == null);
            return freeSlot == null ? false : true;
        }

      

        public bool CheckInventoryRemaining(Dictionary<GameItem, int> shoppingCart)
        {
            var clonedSystem = new InventorySystem(this.InventorySize);
            for(int i =0; i<InventorySize; i++)
            {
                clonedSystem.InventorySlots.Add(new InventorySlot(this.InventorySlots[i].GameItem, this.InventorySlots[i].StackSize));
            }
            foreach(var item in shoppingCart)
            {
                if(!clonedSystem.AddToInventory(item.Key, item.Value)) return false;
            }
           return true;
        }
    }

}
