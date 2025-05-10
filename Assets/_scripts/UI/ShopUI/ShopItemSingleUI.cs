using GameSystems.Inventory;
using GameSystems.ShopSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemSingleUI : MonoBehaviour
{
    [SerializeField] private Image _itemSprite, _cartBackground;
    [SerializeField] private TextMeshProUGUI _itemNameText, _itemAmountText, _itemCountText;
    [SerializeField] private Color _cartBackgroundColor, _cartBackgroundColorGrey;
    [SerializeField] private Color _itemColor = Color.white;
    [SerializeField] private Color _itemColorGrey = Color.grey;
    [SerializeField] private Color _itemTextColor = Color.black;
    [SerializeField] private Color _itemTextColorGrey = Color.grey;
    [SerializeField] private Button _addItemButton, _removeItemButton;
    public ShopKeeperDisplayUI _shopKeeperDisplayUI;
    private ItemSlot _shopSlot;
    public ItemSlot ShopSlot =>_shopSlot;
    private float _markup;
    public float MarkUp => _markup;

    private int _tempAmount;

    private void Awake()
    {
        _itemSprite.sprite = null;
        _itemSprite.preserveAspect = true;
        _itemSprite.color = Color.clear;

        _itemNameText.text = string.Empty;
        _itemAmountText.text = string.Empty;


        _addItemButton?.onClick.AddListener(() =>
        {
            AddItemToCart();
        });
        _removeItemButton?.onClick.AddListener(() =>
        {
            RemoveItemFromCart();
        });

        _shopKeeperDisplayUI = GetComponentInParent<ShopKeeperDisplayUI>();
    }

   
    public void SetItem(ItemSlot slot, float markUp)
    {
        _shopSlot = slot;
        _markup = markUp;
        _tempAmount = slot.StackSize;
        UpdateSlotUI();
    }

    private void UpdateSlotUI()
    {
        if(_shopSlot.ItemData == null) return;
       
        int itemAmount = ShopKeeperDisplayUI.GetModifiedPrice(_shopSlot.GameItem, 1 , _markup) ;
        
        _itemSprite.sprite = _shopSlot.ItemData.Icon;
        _itemNameText.text = $"Item: {_shopSlot.ItemData.DisplayName}";
        _itemCountText.text = $"Amount: {_tempAmount}";
        _itemAmountText.text = $"Price: {itemAmount}";

        if (_tempAmount > 0)
        {
            _itemSprite.color = _itemColor;
            _cartBackground.color = _cartBackgroundColor;
            UpdateTextColor(_itemTextColor);
        }
        else
        {
            _itemSprite.color = _itemColorGrey;
            _cartBackground.color = _cartBackgroundColorGrey;
            UpdateTextColor(_itemTextColorGrey);
        }
    }


    private void UpdateTextColor(Color textColor)
    {
        _itemNameText.faceColor = textColor;
        _itemCountText.faceColor = textColor;
        _itemAmountText.faceColor = textColor;
    }

    private void AddItemToCart()
    {
        if(_tempAmount <= 0) return;
        _tempAmount--;
        _shopKeeperDisplayUI.AddItemToCart(this,1);
        UpdateSlotUI();
    }

    private void RemoveItemFromCart()
    {
        if (_tempAmount == _shopSlot.StackSize) return;
        _tempAmount++;
        _shopKeeperDisplayUI.RemoveFromCart(this, 1);
        UpdateSlotUI();
    }
}
