using GameSystems.Inventory;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace GameSystems.ShopSystem
{
    [CreateAssetMenu(fileName = "ShopItemList", menuName = "Inventory System/Shop System/ShopItemList")]
    public class ShopItemList : ScriptableObject
    {
        [field:SerializeField] private List<ShopItem> _items;
        public List<ShopItem> Items => _items;
        [SerializeField] private int _maxAllowedGold;
        public int MaxAllowedGold => _maxAllowedGold;
        [SerializeField] private float _sellMarkup;
        public float SellMarkup => _sellMarkup;
        [SerializeField] private float _buyMarkup;
        public float BuyMarkup => _buyMarkup;
    }

    [Serializable]
    public struct ShopItem
    {
        public bool purchasesOnly;
        public GameItem Item;
        public int amount;
        public int price;
    }
}