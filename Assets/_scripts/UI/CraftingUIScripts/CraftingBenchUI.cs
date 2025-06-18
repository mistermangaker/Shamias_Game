using GameSystems.Crafting;
using GameSystems.Inventory;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public struct OnWorkbenchScreenRequested : IEvent
{
    public Vector3 requestPosition;
    public InventorySystem buildingInventorySystem;
    public List<InventorySystem> nearbyInventories;
    public InventorySystem playerInventory;
   
}



public class CraftingBenchUI : InventoryDisplayUI
{
    [SerializeField] protected CraftingSlotsUI[] slots;
    [SerializeField] protected GameObject ItemSlotsContainer;
    [SerializeField] protected CraftingSlotsUI toolslot;
    [SerializeField] protected CraftingResultSlotUI resultSlot;
    [SerializeField] protected List<CraftingBenchItemRecipe> itemRecipes;
    [SerializeField] protected GameObject screen;
    //[SerializeField] protected 

    public static CraftingBenchUI Instance { get; private set; }
  
    public bool CraftingScreenIsOpen
    {
        get 
        {
            return screen.activeSelf;
        }
    }
    public void HandleIsOpen()
    {
           Hide();

    }

  

    private Vector3 cachedWorkBenchPosition;
    private const int XSIZE = 4;
    private const int YSIZE = 3;

    protected override void Start()
    {
        Instance = this;
        base.Start();
        
        inventorySystem = new InventorySystem(slots.Length);
        RefreshDisplay();
        
        Hide();
    }

   
    private void DropAllItems()
    {
        foreach (var slot in slotDictionary.Keys)
        {
            if (!slot.IsEmpty)
            {
                EventBus<OnDropItemAtPositionRequested>.Raise(new OnDropItemAtPositionRequested(slot.SlotItem, cachedWorkBenchPosition, slot.ItemAmount));
                slot.ClearSlot();
            }
        }
    }
    public void PopulateCraftingScreen(OnWorkbenchScreenRequested arg)
    {
        
        cachedWorkBenchPosition = arg.requestPosition;
        Show();
    }

    public ItemRecipe recipe;
    private void UpdateCurrentCraftingSuggestion()
    {

        List<ItemData> items = new List<ItemData>();
        foreach (var item in inventorySystem.InventorySlots)
        {
            items.Add(item.ItemData);
        }

        ItemRecipe currentRecipe = ItemRecipe.GenerateItemRecipeFromShapedCraftingGrid(XSIZE, YSIZE, items, toolslot.ItemData);
        recipe = currentRecipe;
        foreach (var itemrecipe in itemRecipes)
        {
            Debug.Log(itemrecipe.CraftingResult.ItemId);
            Debug.Log(itemrecipe.ItemRecipe.ToString());
            Debug.Log(currentRecipe.ToString());
            if(currentRecipe.Equals(itemrecipe.ItemRecipe))
            {
                SetoutputItem(itemrecipe.CraftingResult);
                return;
            }  
        }
        ClearOutput(); 
    }


    private void ClearOutput()
    {
        resultSlot.ClearSlot();
    }
    private void SetoutputItem(ItemData item)
    {
        GameItem newitem = GameItem.DefaultItem(item);
        resultSlot.SetItem(newitem, 1);
    }

    protected void RefreshDisplay()
    {
        inventorySystem.OnInventorySlotChanged += UpdateSlot;
        AssignSlots(inventorySystem, 0);
    }

    public override void SlotClicked(ItemSlotUI slotClicked, MouseButton button = MouseButton.Left)
    {
        if (slotClicked == resultSlot && !resultSlot.IsEmpty)
        {
            if (button == MouseButton.Left)
            {
                if (MouseObjectUI.Instance.IsEmpty)
                {
                    MouseObjectUI.Instance.SetItem(slotClicked.SlotItem, slotClicked.ItemAmount);
                    slotClicked.ClearSlot();
                }
                else if (!MouseObjectUI.Instance.IsEmpty)
                {
                    if (MouseObjectUI.Instance.SlotItem.GameItemData == slotClicked.SlotItem.GameItemData)
                    {
                        MouseObjectUI.Instance.AddToItemStack(slotClicked.ItemAmount);
                    }
                }
                else
                {
                    return;
                }
              
            }
            HandleCrafting();
        }
            base.SlotClicked(slotClicked, button);
        UpdateCurrentCraftingSuggestion();
    }

    private void HandleCrafting()
    {
        foreach(var slot in slotDictionary.Keys)
        {
            if(slot == toolslot||slot==resultSlot) continue;
            slot.RemoveFromItemStack(1);
        }
    }

    // this is really fucking messy but i cant think of a more clever solution currently
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

        InventorySlot toolItemSlot = new InventorySlot();
        InventorySlot resultItemSlot = new InventorySlot();
        inventorySystem.AddAdditionalSlot(toolItemSlot);
        inventorySystem.AddAdditionalSlot(resultItemSlot);
        slotDictionary.Add(toolslot, toolItemSlot);
        slotDictionary.Add(resultSlot, resultItemSlot);
        toolslot.Init(toolItemSlot);
        toolslot.UpdateUI();
        resultSlot.Init(resultItemSlot);
        resultSlot.UpdateUI();
    }

    private void Show()
    {
        screen.SetActive(true);
    }
    private void Hide()
    {
        screen.SetActive(false);
        DropAllItems();
    }
}
