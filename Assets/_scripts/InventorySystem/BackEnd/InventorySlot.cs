using NUnit.Framework.Interfaces;
using System;
using UnityEngine;
namespace GameSystems.Inventory
{
    [Serializable]
    public class InventorySlot : ItemSlot
    {
        //private const string EMPTYITEMNAME = "empty";
       
       //[SerializeField] private ItemData _itemData => _gameItem.ItemData;
        //[SerializeField] private GameItem _gameItem;
       // [SerializeField] private int _stackSize;
        //[SerializeField] private string itemName => _gameItem.ItemData !=null? _gameItem.ItemTypeID : EMPTYITEMNAME;

       // public GameItem GameItem => _gameItem;
       // public ItemData ItemData => _gameItem.ItemData;
       // public int StackSize => _stackSize;
       // public string ItemName => itemName;

        public InventorySlot (GameItem gameItem, int stackSize)
        {
            _gameItem = gameItem;
            //itemName = (gameItem.ItemData  != null) ? gameItem.ItemData.ItemId : EMPTYITEMNAME;
            // _itemData = gameItem.ItemData;
            _stackSize = stackSize;
           
        }


        public InventorySlot()
        {
            ClearSlot();
        }

        //public void ClearSlot()
        //{
        //    //itemName = EMPTYITEMNAME;
        //   // _itemData = null;
        //    _gameItem = default;
        //    _stackSize = -1;
        //}

        //public void AddItem(GameItem item, int amount)
        //{
        //    _gameItem = item;
        //    //itemName = (_gameItem.ItemData != null) ? _gameItem.ItemData.ItemId : EMPTYITEMNAME;
        //   // _itemData = _gameItem.ItemData;
        //    _stackSize = amount;
        //}
       

        //public int GetAmountToAdd(int amount)
        //{
        //    int newAmount = GetSpaceInStack();


        //    return Mathf.Min(newAmount, amount);
        //}

        //public int GetSpaceInStack()
        //{
        //    return (_itemData != null) ? _itemData.MaxStackSize - _stackSize : 0;
        //}
        

        //public void AddToStack(int amount)
        //{
        //    _stackSize += amount;
        //}
        //public void RemoveFromStack(int amount)
        //{
        //    _stackSize -= amount;
            
        //}

        //public void OnBeforeSerialize()
        //{
           
        //}

        //public void OnAfterDeserialize()
        //{
        //    if (GameItem.ItemTypeID == "empty") return;
        //    var database = Resources.Load<ItemDataBase>("DataBase");
        //    GameItem.SetItemData(database.GetItem(GameItem.ItemTypeID));
        //}
    }

}
