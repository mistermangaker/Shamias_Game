using GameSystems.Inventory;
using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public abstract class ItemSlot 
{
    protected const string EMPTYITEMNAME = "empty";
    public UnityAction SlotUpdated;

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
        SlotUpdated?.Invoke();
    }

    public void AddItem(GameItem item, int amount)
    {
        _gameItem = item;
        _stackSize = amount;
        SlotUpdated?.Invoke();
    }


    public int GetAmountToAdd(int amount)
    {
        int newAmount = (ItemData != null) ? ItemData.MaxStackSize - _stackSize : 0;

        return Mathf.Min(newAmount, amount);
    }


    public void AddToStack(int amount)
    {
        _stackSize += amount;
        SlotUpdated?.Invoke();
    }
    public void RemoveFromStack(int amount)
    {
        _stackSize -= amount;
        if (_stackSize <= 0)
        {
            ClearSlot();
        }
        SlotUpdated?.Invoke();
    }
}
