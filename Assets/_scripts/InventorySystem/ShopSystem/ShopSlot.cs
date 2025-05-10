using GameSystems.Inventory;
using System;
using UnityEngine;

namespace GameSystems.ShopSystem
{
    [Serializable]
    public class ShopSlot : ItemSlot
    {
        public ShopSlot(GameItem item, int amount)
        {
            this._gameItem = item;
            this._stackSize = amount;
        }
        public ShopSlot()
        {
            ClearSlot();
        }
    }

}
