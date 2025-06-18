using GameSystems.Inventory;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DynamicInventoryDisplay : InventoryDisplayUI
{
    [SerializeField] protected InventorySlotUI slotPrefab;
    protected override void Start()
    {
        base.Start();
        gameObject.SetActive(false);
    }


    public void RefreshDynamicInventoryDisplay(InventorySystem newInventorySystem, int offset)
    {
        ClearSlots();
        inventorySystem = newInventorySystem;
        if (inventorySystem != null) inventorySystem.OnInventorySlotChanged += UpdateSlot;
        AssignSlots(newInventorySystem, offset);
    }

    public override void AssignSlots(InventorySystem InvToDisplay, int offset)
    {
        slotDictionary = new Dictionary<ItemSlotUI, InventorySlot>();
        if (inventorySystem == null ) return;
        for (int i = offset; i < InvToDisplay.InventorySize; i++)
        {
            var uislot = Instantiate(slotPrefab,transform);
            slotDictionary.Add(uislot, InvToDisplay.InventorySlots[i]);
            uislot.Init(InvToDisplay.InventorySlots[i]);
            uislot.UpdateUI();
        }

    }

    private void ClearSlots()
    {
        foreach(var transform in transform.Cast<Transform>())
        {
            Destroy(transform.gameObject);
        }
        if (slotDictionary != null) slotDictionary.Clear();

    }

    private void OnDisable()
    {
        if (inventorySystem != null) inventorySystem.OnInventorySlotChanged -= UpdateSlot;
    }
}
