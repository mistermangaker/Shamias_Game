using NUnit.Framework.Interfaces;
using System;
using UnityEngine;
using UnityEngine.Events;
namespace GameSystems.Inventory
{
    [Serializable]
    public class InventorySlot : ItemSlot
    {
       
        public InventorySlot (GameItem gameItem, int stackSize)
        {
            _gameItem = gameItem;
            _stackSize = stackSize;
        }


        public InventorySlot()
        {
            ClearSlot();
        }

      
    }

}
