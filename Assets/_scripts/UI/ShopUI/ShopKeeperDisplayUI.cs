using GameSystems.Inventory;
using GameSystems.ShopSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopKeeperDisplayUI : MonoBehaviour
{
    public static ShopKeeperDisplayUI Instance;

    [SerializeField] private GameObject _shopKeeperDisplayWindow;

    [SerializeField] private ShopItemSingleUI shopItemSingleUIPrefab;
    [SerializeField] private ShoppingCartItemUI shoppingCartItemUIPrefab;
    

    [Header("Shopping Cart")]
    [SerializeField] private TextMeshProUGUI _basketTotalText;
    [SerializeField] private TextMeshProUGUI _storeGoldText;
    [SerializeField] private TextMeshProUGUI _playerGoldText;
    [SerializeField] private TextMeshProUGUI _makeSaleButtonText;
    [SerializeField] private Button _makeSaleButton;

    [Header("Item Preview Section")]
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;
    [SerializeField] private Image _itemIcon;

    [Header("panels")]
    [SerializeField] private GameObject _shoppingCartContentPanel;
    [SerializeField] private GameObject _itemListContentPanel;
    [SerializeField] private Button _buyTab, _sellTab;

    private int _basketTotal;

    private ShopSystem _shopSystem;
    private PlayerInventory _playerInventory;

    private Dictionary<GameItem, int> _shoppingCart = new Dictionary<GameItem, int>();
    private Dictionary<GameItem, ShoppingCartItemUI> _shoppingCartUI = new Dictionary<GameItem, ShoppingCartItemUI>();
    private bool _isSelling;
    public bool IsShopScreenOpen
    {
        get
        {
            return _shopKeeperDisplayWindow.activeSelf;
        }
    }

    private void Awake()
    {
        Instance = this;
        HideWindow();
        _buyTab.onClick.AddListener(OnBuyTabPressed);
        _sellTab.onClick.AddListener(OnSellTabPressed);
    }

    public void HandleShopKeeperDisplayOpening(ShopSystem shopSystem, PlayerInventory playerInventory)
    {
        _shopSystem = shopSystem;
        _playerInventory = playerInventory;

        ShowWindow();
        ReFreshDisplay();
    }

    public void HandleShopKeeperDisplayClosing()
    {
        HideWindow();
    }


    public void AddItemToCart(ShopItemSingleUI item, int amount)
    {
        var data = item.ShopSlot.GameItem;
        UpdateItemPreview(data);
        if (!_shoppingCart.ContainsKey(data))
        {
            _shoppingCart.Add(data, amount);
            var ShoppingCartTextObject = Instantiate(shoppingCartItemUIPrefab, _shoppingCartContentPanel.transform);
            _shoppingCartUI.Add(data, ShoppingCartTextObject);
        }
        else
        {
            _shoppingCart[data] += amount;
        }
        var price = GetModifiedPrice(data, amount, item.MarkUp);
        var newString = $"{data.GameItemData.DisplayName} ${price} X {_shoppingCart[data]}";
        
        _shoppingCartUI[data].SetItemText(newString);
        _basketTotal += price;
        _basketTotalText.text = $"Total: ${_basketTotal}";
        if(_basketTotal > 0)
        {
            _basketTotalText.enabled = true;
           // _makeSaleButton.gameObject.SetActive(true);
        }

        CheckCartVsAvailibleMoney();
    }

   
    private void CheckCartVsAvailibleMoney()
    {
        var goldToCheck = _isSelling ? _shopSystem.AvailibleGold : _playerInventory.HeldMoney;
        _basketTotalText.faceColor = _basketTotal > goldToCheck? Color.red : Color.black;

        if (_isSelling || _playerInventory.InventorySystem.CheckInventoryRemaining(_shoppingCart)) return;

        _basketTotalText.text = "Not Enough Room In Inventory";
        _basketTotalText.faceColor =Color.red;
    }

    public static int GetModifiedPrice(GameItem data, int amount, float markUp)
    {
        var basevalue = data.GameItemData.DefaultSellPrice * amount;
        return Mathf.RoundToInt(basevalue + basevalue*markUp);
    }

    public void RemoveFromCart(ShopItemSingleUI item, int amount)
    {
        var data = item.ShopSlot.GameItem;
        if (!_shoppingCart.ContainsKey(data)) return;
        
        _shoppingCart[data] -= amount;

        var price = GetModifiedPrice(data, amount, item.MarkUp);
        var newString = $"{data.GameItemData.DisplayName} ${price} X {_shoppingCart[data]}";
        _shoppingCartUI[data].SetItemText(newString);
        _basketTotal -= price;
        _basketTotalText.text = $"Total: ${_basketTotal}";

        if(_shoppingCart[data] <= 0)
        {
            _shoppingCart.Remove(data);
            var temp = _shoppingCartUI[data].gameObject;
            _shoppingCartUI.Remove(data);
            Destroy(temp);
        }

        if (_basketTotal <= 0)
        {
            _basketTotalText.enabled = false;
           // _makeSaleButton.gameObject.SetActive(false);
            ClearItemPreview();
            return;
        }
        
        CheckCartVsAvailibleMoney();
        
    }


    private void UpdateItemPreview(GameItem item)
    {
        var data = item.GameItemData;
        _itemNameText.text = data.DisplayName;
        _itemDescriptionText.text = data.ItemDescription;
        _itemIcon.sprite = data.Icon;
        _itemIcon.color = Color.white;

    }

    private void ClearItemPreview()
    {
        _itemNameText.text = string.Empty;
        _itemDescriptionText.text = string.Empty;
        _itemIcon.sprite = null;
        _itemIcon.color = Color.clear;
    }
    private void ReFreshDisplay()
    {

        if(_makeSaleButton != null)
        {
            _makeSaleButtonText.text = _isSelling ?  "Sell Items": "Buy Items";
            _makeSaleButton.onClick.RemoveAllListeners();
            if (_isSelling) _makeSaleButton.onClick.AddListener(SellItems);
            else _makeSaleButton.onClick.AddListener(BuyItems);
            
        }


        ClearSlots();
        ClearItemPreview();
        _basketTotalText.text = string.Empty;
        //_makeSaleButton.gameObject.SetActive(false);
       
        _basketTotal = 0;
        
        _playerGoldText.text = $"Player Gold: {_playerInventory.HeldMoney}";
        _storeGoldText.text = $"Shop Gold: {_shopSystem.AvailibleGold}";

        if (!_isSelling) DisplayShopInventory();
        else DisplayPlayerInventory();


    }

    private void ClearSlots()
    {
        _shoppingCart = new Dictionary<GameItem, int>();
        _shoppingCartUI = new Dictionary<GameItem, ShoppingCartItemUI>();
        foreach(var transform  in _itemListContentPanel.transform.Cast<Transform>())
        {
            Destroy(transform.gameObject);
        }
        foreach(var transform1 in _shoppingCartContentPanel.transform.Cast<Transform>())
        {
            Destroy(transform1.gameObject);
        }
    }

    private void HideWindow()
    {
        _shopKeeperDisplayWindow?.SetActive(false);
    }
    private void ShowWindow()
    {
        _shopKeeperDisplayWindow?.SetActive(true);
    }


    private void SellItems()
    {
        if (_basketTotal > _shopSystem.AvailibleGold) return;

        foreach (var item in _shoppingCart)
        {
            _shopSystem.AddToShop(item.Key, item.Value);
            _playerInventory.RemoveItemsFromInventory(item.Key, item.Value);
        }
        _playerInventory.AddMoney(_basketTotal);
        _shopSystem.TryRemoveGold(_basketTotal);
        ReFreshDisplay();
    }
    private void BuyItems()
    {
        if (_basketTotal > _playerInventory.HeldMoney) return;

        if(!_playerInventory.InventorySystem.CheckInventoryRemaining(_shoppingCart) ) return;


        foreach(var item in _shoppingCart)
        {
            
            _shopSystem.PurchaseItem(item.Key, item.Value);
            _playerInventory.AddToInventory(item.Key, item.Value);

            
        }
        _playerInventory.TryRemoveMoney(_basketTotal);
        _shopSystem.TryAddGold(_basketTotal);
        ReFreshDisplay();
    }
    private void DisplayShopInventory()
    {
        foreach(var item in _shopSystem.ShopInventory)
        {
            if(item.IsEmpty) continue;
            ShopItemSingleUI shopItemSingleUI = Instantiate(shopItemSingleUIPrefab, _itemListContentPanel.transform);
            shopItemSingleUI.SetItem(item, _shopSystem.SellMarkup);
        }
    }

    private void DisplayPlayerInventory()
    {


        foreach (var item in _playerInventory.GetAllHeldInventoryItems())
        {
            var tempslot = new ShopSlot(item.Key, item.Value);
           
            ShopItemSingleUI shopItemSingleUI = Instantiate(shopItemSingleUIPrefab, _itemListContentPanel.transform);
            shopItemSingleUI.SetItem(tempslot, _shopSystem.BuyMarkup);
        }
    }

    public void OnBuyTabPressed()
    {
        _isSelling = false;
        ReFreshDisplay();
    }

    public void OnSellTabPressed()
    {
        _isSelling = true;
        ReFreshDisplay();
    }

}
