using GameSystems.Inventory;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace GameSystems.ShopSystem
{
    [Serializable]
    public class ShopSystem
    {
       
        [field:SerializeField] public List<ShopSlot> ShopInventory { get; private set; }
        [SerializeField] public int AvailibleGold {  get; private set; }
        [SerializeField] public float BuyMarkup { get; private set; }
        [SerializeField] public float SellMarkup { get; private set; }

        public ShopSystem() { }
        public ShopSystem(int size, int gold, float buyMarkup, float sellMarkup)
        {
            AvailibleGold = gold;
            BuyMarkup = buyMarkup;
            SellMarkup = sellMarkup;
            SetShopSize(size);
        }

        public void SetGold(int gold)
        {
            AvailibleGold = gold;
        }
        public void TryAddGold(int gold)
        {
            AvailibleGold += gold;
        }
        public void TryRemoveGold(int gold)
        {
            AvailibleGold -= gold;
            if( AvailibleGold < 0 ) {  AvailibleGold = 0; }
        }

        private void SetShopSize(int size)
        {
            ShopInventory = new List<ShopSlot>(size);
            for(int  i =0 ; i < size; i++)
            {
                ShopInventory.Add(new ShopSlot());
            }
        }

        public void AddToShop(GameItem item, int amount)
        {
            if(ContainsItem(item, out ShopSlot slot))
            {
                Debug.Log("yep");
                slot.AddToStack(amount);
                return;
            }
            var freeslot = GetFreeShopSlot();
            freeslot.AddItem(item, amount);

        }

        private ShopSlot GetFreeShopSlot()
        {
            var freeslot = ShopInventory.Find(i => i.ItemData == null);
            if(freeslot == null)
            {
                freeslot = new ShopSlot();
                ShopInventory.Add(freeslot);
            }
            return freeslot;
        }

        public bool ContainsItem(GameItem item, out ShopSlot shopslot)
        {
            shopslot = null;
            foreach(ShopSlot slot in ShopInventory)
            {
                if (slot.GameItem == item)
                {
                    shopslot=slot;
                    return true;
                }
            }
            return false;
        }

        public void PurchaseItem(GameItem item, int amount)
        {
            if(!ContainsItem(item,out ShopSlot slot)) { return; }
            slot.RemoveFromStack(amount);
        }
    }
}
