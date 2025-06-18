using GameSystems.Inventory;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryDisplayUI : MonoBehaviour
{
    [SerializeField] protected InventorySystem inventorySystem;
    protected Dictionary<ItemSlotUI, InventorySlot> slotDictionary;
    public InventorySystem InventorySystem => inventorySystem;
    public Dictionary<ItemSlotUI, InventorySlot> SlotDictionary => slotDictionary;

    private MouseObjectUI mouseInventory;

    protected virtual void Start()
    {
        mouseInventory = MouseObjectUI.Instance;
    }

    public abstract void AssignSlots(InventorySystem InvToDisplay,int offset);

    protected virtual void UpdateSlot(InventorySlot updatedSlot)
    {
        foreach (var slot in slotDictionary)
        {
            if(slot.Value == updatedSlot)
            {
                slot.Key.UpdateSlot(updatedSlot);
            }
        }
    }

    
    public virtual void SlotClicked(ItemSlotUI slotClicked, MouseButton button = MouseButton.Left)
    {
        
        bool canRecievItem = slotClicked.CanRecieveItemPlacementofThisType(mouseInventory.SlotItem);
        
        if (button == MouseButton.Left)
        {
            if (mouseInventory.IsEmpty && !slotClicked.IsEmpty)
            {
                mouseInventory.SetItem(slotClicked.SlotItem, slotClicked.ItemAmount);
                slotClicked.ClearSlot();
            }
            else if (!mouseInventory.IsEmpty && slotClicked.IsEmpty && canRecievItem)
            {
                slotClicked.SetItem(mouseInventory.SlotItem, mouseInventory.StackSize);
                mouseInventory.ClearSlot();
            }
            else if (!mouseInventory.IsEmpty && !slotClicked.IsEmpty && canRecievItem)
            {

                if (mouseInventory.SlotItem.GameItemData == slotClicked.SlotItem.GameItemData)
                {
                    TryAddItem(slotClicked, mouseInventory.StackSize);
                }
                else if (mouseInventory.SlotItem.GameItemData != slotClicked.SlotItem.GameItemData)
                {
                    SwapSlots(slotClicked);
                }
            }
        }
        if (button == MouseButton.Right)
        {
            if (mouseInventory.IsEmpty && !slotClicked.IsEmpty)
            {
                if (slotClicked.ItemAmount > 1)
                {
                    int amount = slotClicked.ItemAmount / 2;
                    mouseInventory.SetItem(slotClicked.SlotItem, amount);
                    slotClicked.RemoveFromItemStack(amount);
                }
                else
                {
                    mouseInventory.SetItem(slotClicked.SlotItem, slotClicked.ItemAmount);
                    slotClicked.ClearSlot();
                }
            }
            else if (!mouseInventory.IsEmpty && slotClicked.IsEmpty && canRecievItem)
            {
                slotClicked.SetItem(mouseInventory.SlotItem, 1);
                mouseInventory.RemoveFromItemStack(1);
            }
            else if (!mouseInventory.IsEmpty && !slotClicked.IsEmpty && canRecievItem)
            {

                if (mouseInventory.SlotItem.GameItemData == slotClicked.SlotItem.GameItemData)
                {
                    TryAddItem(slotClicked, 1);
                }
                if (mouseInventory.SlotItem.GameItemData != slotClicked.SlotItem.GameItemData)
                {
                    if (mouseInventory.StackSize == 1)
                    {
                        SwapSlots(slotClicked);
                    }
                }
            }
        }

    }
    private void TryAddItem(ItemSlotUI slotClicked, int amount)
    {
        int amountToAdd = slotClicked.AssignedInventorySlot.GetAmountToAdd(amount);
        if (amountToAdd > 0)
        {
            slotClicked.AddToItemStack(amountToAdd);
            mouseInventory.RemoveFromItemStack(amountToAdd);
        }
    }

    private void SwapSlots(ItemSlotUI slotClicked)
    {
        InventorySlot temp = new InventorySlot(mouseInventory.SlotItem, mouseInventory.StackSize);
        mouseInventory.SetItem(slotClicked.SlotItem, slotClicked.ItemAmount);
        slotClicked.SetItem(temp.GameItem, temp.StackSize);
    }

}
