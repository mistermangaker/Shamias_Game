using GameSystems.Inventory;
using GameSystems.ShopSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ItemSlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected TextMeshProUGUI quantityText;

    [SerializeField] protected Image itemRenderer;

    

    [SerializeField] protected GameObject hoverVisual;

    [SerializeField] protected InventorySlot assignedInventorySlot;
    public InventorySlot AssignedInventorySlot => assignedInventorySlot;

    public GameItem SlotItem => assignedInventorySlot.GameItem;
    public ItemData ItemData => assignedInventorySlot.ItemData;

    public int ItemAmount => assignedInventorySlot.StackSize;


    protected MouseObjectUI mouseObject;
    public InventoryDisplayUI parentDisplay { get; protected set; }

    public bool IsEmpty
    {
        get
        {
            return SlotItem.GameItemData == null;
        }
    }

    public abstract bool CanRecieveItemPlacementofThisType(GameItem item);




    private void Awake()
    {
        mouseObject = MouseObjectUI.Instance;
        UpdateUI();
    }

    public virtual void UpdateSlot(InventorySlot slot)
    {
        if (slot.StackSize <= 0) { ClearSlot(); return; }
        assignedInventorySlot.AddItem(slot.GameItem, slot.StackSize);
        UpdateUI();
    }

    public virtual void UpdateUI()
    {
       
        if (SlotItem.GameItemData != null)
        {
            quantityText.text = (assignedInventorySlot.StackSize > 1) ? AssignedInventorySlot.StackSize.ToString() : string.Empty;
            itemRenderer.sprite = SlotItem.GameItemData.Icon;
            itemRenderer.color = Color.white;

        }
        else
        {

            quantityText.text = string.Empty;
            itemRenderer.sprite = null;
            itemRenderer.color = Color.clear;
        }
    }


    public virtual void SetItem(GameItem item, int amount)
    {
        assignedInventorySlot.AddItem(item, amount);
        UpdateUI();
    }


    public virtual void AddToItemStack(int amount)
    {
        assignedInventorySlot.AddToStack(amount);
        UpdateUI();
    }
    public virtual void RemoveFromItemStack(int amount)
    {
        assignedInventorySlot.RemoveFromStack(amount);
        if (assignedInventorySlot.StackSize <= 0)
        {
            ClearSlot();
            return;
        }
        UpdateUI();
    }

    public virtual void ClearSlot()
    {
        assignedInventorySlot.ClearSlot();
        UpdateUI();
    }

    public virtual void HoverSlot()
    {
        hoverVisual.SetActive(true);
        UpdateUI();
    }
    public virtual void UnHoverSlot()
    {
        hoverVisual.SetActive(false);
        UpdateUI();
    }

  
    public abstract void OnPointerClick(PointerEventData eventData);
  

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        HoverSlot();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        UnHoverSlot();
    }
    public virtual void Init(InventorySlot slot)
    {
        assignedInventorySlot = slot;
        assignedInventorySlot.SlotUpdated += UpdateUI;
        parentDisplay = GetComponentInParent<InventoryDisplayUI>();
        UpdateUI();
    }
}
