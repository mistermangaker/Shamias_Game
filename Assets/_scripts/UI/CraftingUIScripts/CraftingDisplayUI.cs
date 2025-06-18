using GameSystems.Inventory;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class CraftingDisplayUI : InventoryDisplayUI
{
    [SerializeField] protected CraftingSlotsUI[] slots;
    [SerializeField] protected GameObject ItemSlotsContainer;
    
    protected override void Start()
    {
        base.Start();
        inventorySystem = new InventorySystem(slots.Length);
        RefreshDisplay();
    }

    public override void AssignSlots(InventorySystem InvToDisplay, int offset)
    {
        slotDictionary = new Dictionary<ItemSlotUI, InventorySlot>();
        if (inventorySystem == null) return;
        for (int i = offset; i < slots.Length; i++)
        {
            slotDictionary.Add(slots[i], InvToDisplay.InventorySlots[i]);
            slots[i].Init(InvToDisplay.InventorySlots[i]);
            slots[i].UpdateUI();
        }
    }

    protected void RefreshDisplay()
    {
        inventorySystem.OnInventorySlotChanged += UpdateSlot;
        AssignSlots(inventorySystem,0);
    }

  
}
