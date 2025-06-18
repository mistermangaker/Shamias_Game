using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameSystems.Inventory;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.PointerEventData;
using System;
using UnityEngine.Events;


public class InventorySlotUI : ItemSlotUI, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    //[SerializeField] private TextMeshProUGUI quantityText;
    //[SerializeField] private Image itemRenderer;
    //[SerializeField] private GameObject selectedItemRenderer;
    //[SerializeField] private GameObject hoverVisual;
    //[SerializeField] private InventorySlot assignedInventorySlot;

    //private MouseObjectUI mouseObject;

    //public InventorySlot AssignedInventorySlot => assignedInventorySlot;
    [SerializeField] protected GameObject selectedItemRenderer;
    public static UnityAction<GameItem> OnHoverRequested;
    public static UnityAction OnHoverExited;

   // public GameItem SlotItem => assignedInventorySlot.GameItem;

   // public int ItemAmount => assignedInventorySlot.StackSize;

   // public InventoryDisplayUI parentDisplay { get; private set; }

    private bool _isSelected=false;

    private bool _pointerHasEntered=false;

    //public bool IsEmpty
    //{
    //    get
    //    {
    //        return SlotItem.GameItemData == null;
    //       // return ItemData == null;
    //    }
    //}


    private void Awake()
    {
        UpdateUI();
        DeselectSlot();
        selectedItemRenderer.SetActive(false);
    }
    private void Start()
    {
        mouseObject = MouseObjectUI.Instance;
    }


    private float hoverTextTimer;
    [SerializeField] private static float hoverTextDelay =0.4f;
    private void Update()
    {
        if (!IsEmpty && _pointerHasEntered)
        {
            hoverTextTimer += Time.deltaTime;
            if(hoverTextTimer > hoverTextDelay)
            {
                ResetTimer();
                OnHoverRequested?.Invoke(SlotItem);
            }
        }
    }

    private void ResetTimer()
    {
        hoverTextTimer = 0;
        _pointerHasEntered = false;
    }
    //public void Init(InventorySlot slot)
    //{
    //    assignedInventorySlot = slot;
    //    parentDisplay = GetComponentInParent<InventoryDisplayUI>();
    //    UpdateUI();
    //}

    //public void ClearSlot()
    //{
    //    assignedInventorySlot.ClearSlot();
    //    UpdateUI();
    //}


    //public void ToggleHightLight()
    //{
    //    selectedItemRenderer.SetActive(!selectedItemRenderer.activeSelf);
    //    //selectedItemRenderer.enabled = !selectedItemRenderer.enabled;
    //}

    //public void UpdateSlot(InventorySlot slot)
    //{
    //    if (slot.StackSize <= 0) { ClearSlot(); return; }
    //    assignedInventorySlot.AddItem(slot.GameItem, slot.StackSize);
    //    UpdateUI();
    //}

    //public void SetItem(GameItem item, int amount)
    //{
    //    assignedInventorySlot.AddItem(item, amount);
    //    UpdateUI();
    //}


    //public void AddToItemStack(int amount)
    //{
    //    assignedInventorySlot.AddToStack(amount);
    //    UpdateUI();
    //}
    //public void RemoveFromItemStack(int amount)
    //{
    //    assignedInventorySlot.RemoveFromStack(amount);
    //    if (assignedInventorySlot.StackSize <= 0)
    //    {
    //        ClearSlot();
    //        return;
    //    }
    //    UpdateUI();
    //}
    public virtual void ToggleHightLight()
    {
        selectedItemRenderer.SetActive(!selectedItemRenderer.activeSelf);
    }


    public override void UpdateUI()
    {
        if (_isSelected) quantityText.faceColor = Color.white;
            else quantityText.faceColor = Color.black;
        base.UpdateUI();
        //if (SlotItem.GameItemData != null)
        //{
        //    quantityText.text = (assignedInventorySlot.StackSize > 1) ? AssignedInventorySlot.StackSize.ToString() : string.Empty;
        //    itemRenderer.sprite = SlotItem.GameItemData.Icon;
        //    itemRenderer.color = Color.white;

        //}
        //else
        //{

        //    quantityText.text = string.Empty;
        //    itemRenderer.sprite = null;
        //    itemRenderer.color = Color.clear;
        //}
    }

 

    //public void HoverSlot()
    //{
    //    hoverVisual.SetActive(true);
    //    UpdateUI();
    //}
    public override void UnHoverSlot()
    {
        //hoverVisual.SetActive(false);
        ResetTimer();
        base.UnHoverSlot();
        //UpdateUI();
    }

    public void SelectSlot()
    {
        _isSelected = true;
        HoverSlot();
    }
    public void DeselectSlot()
    {
        _isSelected = false;
       
        UnHoverSlot();
    }

    public void AssignSlotDisplay(GameItem item , int amount)
    {
        int slotDisplayAmount = (assignedInventorySlot.StackSize != -1)? assignedInventorySlot.StackSize + amount: amount;
        quantityText.text = slotDisplayAmount.ToString();
        itemRenderer.sprite = item.GameItemData.Icon;
        itemRenderer.color = Color.white;
    }
    

    public override void OnPointerClick(PointerEventData eventData)
    {
        MouseButton button = (eventData.button == PointerEventData.InputButton.Right) ? MouseButton.Right : MouseButton.Left;
        parentDisplay?.SlotClicked(this, button);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        _pointerHasEntered = true;  
        if (mouseObject.DraggingSlots && (eventData.button == PointerEventData.InputButton.Right || eventData.button == PointerEventData.InputButton.Left))

        {
            if (MouseObjectUI.Instance.SlotItem.GameItemData == SlotItem.GameItemData || IsEmpty)
            {
                MouseButton button = eventData.button == PointerEventData.InputButton.Right ? MouseButton.Right : MouseButton.Left;
                MouseObjectUI.Instance.AddSlotUI(this, button);
                SelectSlot();
            }

        }
        else HoverSlot();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        _pointerHasEntered = false;
        OnHoverExited?.Invoke();
        if (mouseObject.DraggingSlots && (MouseObjectUI.Instance.SlotItem.GameItemData == SlotItem.GameItemData || IsEmpty)) return;

        DeselectSlot();

    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right || eventData.button == PointerEventData.InputButton.Left)
        {
            if (MouseObjectUI.Instance.SlotItem.GameItemData != null)
            {
                MouseButton button = eventData.button == PointerEventData.InputButton.Right ? MouseButton.Right : MouseButton.Left;
                MouseObjectUI.Instance.AddSlotUI(this, button);
                SelectSlot();
            }

        }
        
    }

    public override bool CanRecieveItemPlacementofThisType(GameItem item)
    {
        return true;
    }

    
}
