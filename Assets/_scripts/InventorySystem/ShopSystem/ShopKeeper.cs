using GameSystems.Inventory;
using GameSystems.SaveLoad;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameSystems.ShopSystem
{
    public class ShopKeeper : MonoBehaviour, IInteractable , IBind<InventorySavaData>
    {
        [SerializeField] private ShopItemList _shopItemsHeld;
        [field:SerializeField] private ShopSystem _shopSystem;
        public UnityAction<IInteractable> OnInteraction { get; set; }
        public UnityAction<IInteractable> OnInteractionEnd { get; set; }

        public static UnityAction<ShopSystem, PlayerInventory> OnShopWindowRequested;
        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
        [SerializeField] private InventorySavaData _shopSystemSaveData;
        private void Awake()
        {
            _shopSystem = new ShopSystem(_shopItemsHeld.Items.Count, _shopItemsHeld.MaxAllowedGold,_shopItemsHeld.BuyMarkup ,_shopItemsHeld.SellMarkup);

            foreach (var item in _shopItemsHeld.Items)
            {
                //Debug.Log(item.Item.ItemTypeID);
                _shopSystem.AddToShop(item.Item, item.amount);
            }
        }

        public bool Interact(InteractionAttempt interactionAttempt)
        {
            if (interactionAttempt.Intent == InteractionIntent.Interact)
            {
                OnShopWindowRequested?.Invoke(_shopSystem, PlayerInventory.Instance);
                return true;
            }
            return false;
        }

        public void OnInteractingEnd()
        {

        }

        public void Bind(InventorySavaData data)
        {
            _shopSystemSaveData.itemSaveDatas.Clear();
            data.Id = Id;
            _shopSystemSaveData = data;
            
            if (data.heldMoney == -1) _shopSystemSaveData.heldMoney = _shopSystem.AvailibleGold;
            else _shopSystem.SetGold(data.heldMoney);


            foreach (var item in data.itemSaveDatas)
            {
                Debug.Log(item.name);
            }

            if (data.itemSaveDatas.Count > 0)
            {
                _shopSystem = new ShopSystem(data.itemSaveDatas.Count, data.heldMoney, _shopItemsHeld.BuyMarkup, _shopItemsHeld.SellMarkup);
                
                foreach (var item in data.GetAllItems())
                {
                    _shopSystem.AddToShop(item.item, item.amount);
                }
                //for (int i = 0; i < data.itemSaveDatas.Count; i++)
                //{

                //    GameItem gameItem = _shopSystemSaveData.itemSaveDatas[i].item;
                //    ItemData item = DataBaseManager.Instance.ItemDataBase.GetItem(_shopSystemSaveData.itemSaveDatas[i].name);
                //    if (item != null)
                //    {
                //        gameItem.SetItemData(item);
                //        _shopSystem.AddToShop(gameItem, _shopSystemSaveData.itemSaveDatas[i].amount);
                //    }
                   
                //}
            }
           

        }

        public void Save(ref InventorySavaData data)
        {
            data.itemSaveDatas.Clear();
            foreach (var item in _shopSystem.ShopInventory)
            {
                if(item.GameItem.ItemTypeID == GameItem.EmptyItemData) continue;
                data.Add(item.GameItem, item.StackSize);
                
            }
            data.heldMoney = _shopSystem.AvailibleGold;
           
            data.Id = Id;
        }
    }
}

