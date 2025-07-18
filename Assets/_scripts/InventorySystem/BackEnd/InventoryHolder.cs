using GameSystems.SaveLoad;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using GameSystems.BuildingSystem;
using System.Linq;
using GameSystems.ShopSystem;
using System.Collections;

namespace GameSystems.Inventory
{
    [System.Serializable]
    public class InventorySavaData :ISaveable
    {
        
        
        [SerializeField] public List<ItemSaveData> itemSaveDatas = new List<ItemSaveData>();

        public List<ItemStack> GetAllItems()
        {
            List<ItemStack> items = new List<ItemStack>();
            for (int i = 0; i < itemSaveDatas.Count; i++)
            {
                ItemData data = GetItemDataAtPosition(i);
                if (data != null)
                {
                    GameItem item = GetGameItemAtPosition(i);
                    ItemStack stack = new ItemStack(item, itemSaveDatas[i].amount);
                    items.Add(stack);
                }
                else
                {
                    Debug.Log("item is null");
                }
                
                //Debug.Log(itemSaveDatas[i].name);
                //GameItem gameItem = itemSaveDatas[i].item;
                //ItemData item = DataBase.GetItem(itemSaveDatas[i].name);
                //Debug.Log(item.ItemId);
                //    //DataBaseManager.Instance.ItemDataBase.GetItem(itemSaveDatas[i].name);
                //if (item != null)
                //{
                //    Debug.Log("item not broke");
                //    gameItem.SetItemData(item);
                //    items.Add(new ItemStack(gameItem, itemSaveDatas[i].amount));
                //}
            }
            return items;
        }
       
        public ItemData GetItemDataAtPosition(int position)
        {
            ItemData item = DataBase.GetItem(itemSaveDatas[position].name);
            if (item == null) { return null; }
            return item;
        }
        public GameItem GetGameItemAtPosition(int position)
        {
            GameItem gameItem = itemSaveDatas[position].item;
            ItemData item = DataBase.GetItem(itemSaveDatas[position].name);
            if (item != null)
            {
                gameItem.SetItemData(item);
                
            }
            return gameItem;
        }

        public ItemStack GetItemStackAtPosition(int position)
        {
            GameItem item = GetGameItemAtPosition(position);
            int amount = itemSaveDatas[position].amount;
            if(item != null)
            {
                return new ItemStack(item, amount);
            }
            return null;
        }



        public int heldMoney = -1;
        [field: SerializeField] public SerializableGuid Id { get; set; }

        public void Add(string name , int amount)
        {
            itemSaveDatas.Add (new ItemSaveData(name, amount));
        }
        public void Add(GameItem item, int amount)
        {
            itemSaveDatas.Add(new ItemSaveData(item, amount));
        }
    }

    public class ItemStack
    {
        public GameItem item;
        public int amount;

        public ItemStack(GameItem item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }
    }

    [Serializable]
    public class ItemSaveData
    {
        public GameItem item;
        public string name;
        public int amount;

        public ItemSaveData(GameItem item, int amount)
        {
            this.item = item;
            this.name = item.GameItemData !=null? item.ItemTypeID : "empty";
            this.amount = amount;
        }

        public ItemSaveData(string name, int amount)
        {
            item = default;
            this.name = name;
            this.amount = amount;
        }
    }

    [System.Serializable]
    public class InventoryHolder : MonoBehaviour, IBind<InventorySavaData>
    {
        [Header("Inventory")]
        [SerializeField] private int InventorySize;
        [SerializeField] protected int offset = 0;
        public int Offset => offset;

        [SerializeField] protected InventorySystem _inventorySystem;
        public InventorySystem InventorySystem => _inventorySystem;

        [SerializeField] private InventorySavaData _inventoryData;


        [field:SerializeField] public SerializableGuid Id { get ; set ; } = SerializableGuid.NewGuid();

        public static UnityAction<InventorySystem, int> OnDynamicInventoryDisplayRequested;
        public UnityAction OnInventoryChanged;

        [SerializeField] protected int _heldMoney;
        public int HeldMoney => _heldMoney;

        public Dictionary<GameItem, int> GetAllHeldInventoryItems()
        {
            return _inventorySystem.GetAllHeldInventoryItems();
        }

        public void AddMoney(int amount)
        {
            _heldMoney += amount;
        }
        public bool TryRemoveMoney(int amount)
        {
            int newAmount = _heldMoney - amount;
            if (newAmount < 0) return false;
            _heldMoney = newAmount;
            return true;
        }
        public void RemoveItemsFromInventory(GameItem item, int amount)
        {
            _inventorySystem.RemoveItemsFromInventory(item, amount);
        }


        protected virtual void Awake()
        {
            _inventorySystem = new InventorySystem(InventorySize);
            _inventoryData = new InventorySavaData();
        }

        public void Bind(InventorySavaData data)
        {
            _inventoryData.itemSaveDatas.Clear();
            _inventoryData = data;
            data.Id = Id;
            if (data.heldMoney == -1) data.heldMoney = _heldMoney;
            else _heldMoney = data.heldMoney;
            for (int i = 0; i < _inventorySystem.InventorySize; i++)
            {
                _inventoryData.Add(InventorySystem.InventorySlots[i].GameItem, InventorySystem.InventorySlots[i].StackSize);
            }
           
            for (int i = 0; i < _inventorySystem.InventorySize; i++)
            {
                GameItem item = _inventoryData.GetGameItemAtPosition(i);
                _inventorySystem.SetSlot(item, _inventoryData.itemSaveDatas[i].amount, i);
               
            }
            OnInventoryChanged?.Invoke();
        }

        public void Save(ref InventorySavaData data)
        {
            data.itemSaveDatas.Clear();
            for (int i = 0; i < _inventorySystem.InventorySize; i++)
            {
                
                data.Add(InventorySystem.InventorySlots[i].GameItem, InventorySystem.InventorySlots[i].StackSize);
            }
            data.heldMoney = _heldMoney;
            data.Id = Id;
        }

        
    }

}
