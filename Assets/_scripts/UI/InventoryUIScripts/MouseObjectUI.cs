using GameSystems.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class MouseObjectUI : MonoBehaviour
{
    public static MouseObjectUI Instance;
    [SerializeField] Image ItemSprite;
    [SerializeField] TextMeshProUGUI ItemCountText;
    [SerializeField] private GameObject ItemContainer;

    public InventorySlot InventorySlot;
    public int StackSize
    {
        get
        {
            return InventorySlot.StackSize;
        }
    }

    public GameItem SlotItem
    {
        get
        {
            return InventorySlot.GameItem;
        }
    }


    [SerializeField] public List<InventorySlotUI> selectedSlots;

    public bool DraggingSlots = false;
    private bool HandlingSlots = false;

    [SerializeField] private Vector3 offset = new Vector3(50, -50, 0);

    public ItemData ItemData
    {
        get
        {
            return InventorySlot.ItemData;
        }
    }
    public bool IsEmpty
    {
        get
        {
            return SlotItem.GameItemData == null;
           //return InventorySlot.ItemData == null;
        }
    }

   // private Transform _playerTransform;
    private Canvas canvas;

    private void Awake()
    {
        Instance = this;
        InventorySlot = new InventorySlot();
        canvas = GetComponentInParent<Canvas>();
        UpdateUI();
    }

    private void Start()
    {
        PlayerInputManager.OnPause += DropItemOnPause;
        InventorySlot.SlotUpdated += UpdateUI;
    }

    private void OnDestroy()
    {
        PlayerInputManager.OnPause -= DropItemOnPause;
        InventorySlot.SlotUpdated -= UpdateUI;
    }
    private void DropItemOnPause()
    {
        DropItem(StackSize);
    }

    private void ClearAllSlots()
    {
        if (HandlingSlots) return;
        HandlingSlots = true;
        foreach (var slot in selectedSlots)
        {
            slot.DeselectSlot();
        }
        selectedSlots.Clear();
        HandlingSlots = false;
        
    }

    private void FillSlots(int AmountToAddPerSlot)
    {
        if(HandlingSlots) return;
        HandlingSlots = true;
        foreach (InventorySlotUI slot in selectedSlots)
        {
            if (StackSize > 0)
            {
                if (slot.SlotItem.GameItemData != null)
                {
                    int amountSlotCanTake = slot.AssignedInventorySlot.GetAmountToAdd(AmountToAddPerSlot);
                    if (amountSlotCanTake > 0)
                    {
                        slot.AddToItemStack(amountSlotCanTake);
                        InventorySlot.RemoveFromStack(amountSlotCanTake);
                    }
                }
                else
                {
                    slot.SetItem(SlotItem, AmountToAddPerSlot);
                    InventorySlot.RemoveFromStack(AmountToAddPerSlot);
                }

                if (StackSize <= 0)
                {
                    ClearSlot();
                }

            }
            slot.DeselectSlot();
        }
        selectedSlots.Clear();
        UpdateUI();
        HandlingSlots = false;
    }


    public void AddSlotUI(InventorySlotUI slot, MouseButton mouseButton = MouseButton.Left)
    {
        if(SlotItem.GameItemData != null && !selectedSlots.Contains(slot))
        {
            selectedSlots.Add(slot);
            DraggingSlots = true;
            AssignDisplaySlotsForDragging(mouseButton);
        }
    }

    

    public void AssignDisplaySlotsForDragging(MouseButton mouseButton)
    {
        if (selectedSlots.Count > 1)
        {
            int stacksize = StackSize;
            int AmountToAddPerSlot = 1;
            if (mouseButton == MouseButton.Left)
            {
                AmountToAddPerSlot = stacksize / selectedSlots.Count;
                if (AmountToAddPerSlot <= 1) AmountToAddPerSlot = 1;
            }

            foreach (InventorySlotUI slotUI in selectedSlots)
            {
                int amountToAdd = (slotUI.IsEmpty) ? AmountToAddPerSlot : slotUI.AssignedInventorySlot.GetAmountToAdd(AmountToAddPerSlot);

                slotUI.AssignSlotDisplay(SlotItem, amountToAdd);
            }
            ItemCountText.text = (stacksize % selectedSlots.Count).ToString();
            ItemCountText.faceColor = Color.white;
        }
        
    }


    public void ClearSlot()
    {
        InventorySlot.ClearSlot();
        ClearAllSlots();
        UpdateUI();
    }
    public void AddToItemStack(int amount)
    {
        InventorySlot.AddToStack(amount);
        UpdateUI();
    }
    public void RemoveFromItemStack(int amount)
    {
        InventorySlot.RemoveFromStack(amount);
        if(InventorySlot.StackSize <= 0)
        {
            ClearSlot();
            return;
        }
        UpdateUI();
    }

    public void SetItem(GameItem item, int amount)
    {
        InventorySlot.AddItem(item, amount);
        UpdateUI();
    }

    


    public void UpdateUI()
    {
        if(SlotItem.GameItemData != null)
        {
            ItemSprite.color = Color.white;
            ItemSprite.sprite = SlotItem.GameItemData.Icon;
            ItemCountText.text = InventorySlot.StackSize.ToString();    
        }
        else
        {
            ItemSprite.color = Color.clear;
            ItemCountText.text = string.Empty;
        }
        if (!DraggingSlots) ItemCountText.faceColor = Color.black;
        
    }


    private void DropItem(int amount)
    {
        //if (SlotItem.GameItemData == null) return;
   
        //EventBus<OnDropItemAtPositionRequested>.Raise(new OnDropItemAtPositionRequested(SlotItem, PlayerController.Instance.transform.position, amount));

        //RemoveFromItemStack(amount);
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }


    //ToDo refactor this tho work with a future fully integrated input system;
    private void Update()
    {
        transform.position = Input.mousePosition + (offset / canvas.scaleFactor);
        if(DraggingSlots)
        {
            if (Mouse.current.rightButton.wasReleasedThisFrame)
            {
                DraggingSlots = false;
                if (selectedSlots.Count > 1)
                {
                    FillSlots(1);
                }
                else
                {
                    ClearAllSlots();
                }
                
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                DraggingSlots = false;
                if (selectedSlots.Count > 1)
                {
                    int AmountToAddPerSlot = StackSize / selectedSlots.Count;
                    if (AmountToAddPerSlot <= 1) AmountToAddPerSlot = 1;

                    FillSlots(AmountToAddPerSlot);
                }
                else
                {
                    ClearAllSlots();
                }
                
            }
        }
        if(SlotItem.GameItemData != null && !HandlingSlots && !DraggingSlots && !IsPointerOverUIObject())
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                DropItem(StackSize);
            }
            else if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                DropItem(1);
            }
        }

    }
}
