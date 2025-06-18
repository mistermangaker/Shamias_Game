using GameSystems.Inventory;
using UnityEngine;
using System.Collections.Generic;

public class StaticInventoryDisplay : InventoryDisplayUI
{
    [SerializeField] protected InventoryHolder inventoryHolder;
    [SerializeField] protected InventorySlotUI[] slots;

    protected override void Start()
    {
        base.Start();
        RefreshStaticDisplay();

    }
    public override void AssignSlots(InventorySystem InvToDisplay, int offset)
    {
        slotDictionary = new Dictionary<ItemSlotUI, InventorySlot>();
        for (int i = 0; i < slots.Length; i++)
        {
            slotDictionary.Add(slots[i], inventorySystem.InventorySlots[i]);
            slots[i].Init(inventorySystem.InventorySlots[i]);
        }
    }

    private void RefreshStaticDisplay()
    {
        if (inventoryHolder != null)
        {
            inventorySystem = inventoryHolder.InventorySystem;
            inventorySystem.OnInventorySlotChanged += UpdateSlot;
            inventoryHolder.OnInventoryChanged += (() => { AssignSlots(inventorySystem, 0); });
        }
        else
        {
            Debug.LogWarning($"No secondaryInventorySystem assigned to {this.gameObject}");
        }
        AssignSlots(inventorySystem, 0);
    }
}
