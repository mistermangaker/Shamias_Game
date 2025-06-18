using GameSystems.Inventory;
using GameSystems.SaveLoad;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameSystems.ShopSystem
{
    public class ShopKeeper : MonoBehaviour, IInteractable, IBind<InventorySavaData>
    {
        [SerializeField] private HighlightableSprite sprite;
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
                GameItem itemToAdd = item.Item;
                if(item.BasicItem !=null) itemToAdd = GameItem.DefaultItem(item.BasicItem);
                //Debug.Log(item.Item.ItemTypeID);
                _shopSystem.AddToShop(itemToAdd, item.amount);
            }
        }

        private void OnEnable()
        {
            sprite.OnHoverOver += HandleHovering;
            sprite.OnHoverOut += HandleUnhovering;
        }
       
        private void OnDisable()
        {
            sprite.OnHoverOver -= HandleHovering;
            sprite.OnHoverOut -= HandleUnhovering;
        }

        private void HandleHovering()
        {
            EventBus<OnIteractableHovered>.Raise(new OnIteractableHovered
            {
                interactable = this,
                name = "shop",
                interactionPosition = transform.position,
            });
        }
        private void HandleUnhovering()
        {
            EventBus<OnIteractableUnHovered>.Raise(new OnIteractableUnHovered());
        }
       

        public bool Interact(InteractionAttempt interactionAttempt)
        {
            if (interactionAttempt.Intent == InteractionIntent.Interact)
            {
                EventBus<OnShopScreenRequested>.Raise(new OnShopScreenRequested {Shop = _shopSystem });
                //OnShopWindowRequested?.Invoke(_shopSystem, PlayerInventory.Instance);
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


            if (data.itemSaveDatas.Count > 0)
            {
                _shopSystem = new ShopSystem(data.itemSaveDatas.Count, data.heldMoney, _shopItemsHeld.BuyMarkup, _shopItemsHeld.SellMarkup);
                
                foreach (var item in data.GetAllItems())
                {
                    _shopSystem.AddToShop(item.item, item.amount);
                }
               
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


        public virtual bool CanAcceptInteractionType(InteractionAttempt interactionAttempt)
        {
            if(interactionAttempt.Intent == InteractionIntent.Interact) return true;
            return false;
        }
    }
}

