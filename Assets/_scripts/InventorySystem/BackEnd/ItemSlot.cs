using GameSystems.Inventory;
using System;
using UnityEngine;

[Serializable]
public abstract class ItemSlot 
{
    protected const string EMPTYITEMNAME = "empty";

    
    [SerializeField] protected GameItem _gameItem;
    [SerializeField] protected int _stackSize;
    [SerializeField] protected string itemName => _gameItem.GameItemData != null ? _gameItem.ItemTypeID : EMPTYITEMNAME;

    public GameItem GameItem => _gameItem;
    public ItemData ItemData => _gameItem.GameItemData;
    public int StackSize => _stackSize;
    public string ItemName => itemName;

    public bool IsEmpty
    {
        get
        {
            return ItemData == null;
        }
    }


    public void ClearSlot()
    {
        //itemName = EMPTYITEMNAME;
        // _itemData = null;
        _gameItem = default;
        _stackSize = -1;
    }

    public void AddItem(GameItem item, int amount)
    {
        _gameItem = item;
        _stackSize = amount;
    }


    public int GetAmountToAdd(int amount)
    {
        int newAmount = (ItemData != null) ? ItemData.MaxStackSize - _stackSize : 0;

        return Mathf.Min(newAmount, amount);
    }


    public void AddToStack(int amount)
    {
        _stackSize += amount;
    }
    public void RemoveFromStack(int amount)
    {
        _stackSize -= amount;
        if (_stackSize <= 0)
        {
            ClearSlot();
        }
    }
}
