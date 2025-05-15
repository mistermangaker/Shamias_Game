using GameSystems.Inventory;
using GameSystems.SaveLoad;
using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class DroppedItemSaveData : ISaveable
{
    [field: SerializeField] public List<DroppedItems> droppedItems = new List<DroppedItems>();
    [field: SerializeField] public List<SerializableGuid> collectedWorldSpawnItems = new List<SerializableGuid>();
    [field: SerializeField] public SerializableGuid Id { get; set; }

   

    public void AddItem(SerializableGuid id, GameItem item, Vector3 poisition, int amount)
    {
        if(!Contains(id)) droppedItems.Add(new DroppedItems(id, item, poisition, amount));
    }

    public void RemoveItem(SerializableGuid id)
    {
        foreach (var droppedItem in droppedItems)
        {
            if(droppedItem.id == id)
            {
                droppedItems.Remove(droppedItem);
                return;
            }
        }
    }

    public bool Contains(SerializableGuid id)
    {
        foreach (var item in droppedItems)
        {
            if(item.id == id) return true;
        }
        return false;
    }
}
[Serializable]
public struct DroppedItems
{
    public SerializableGuid id;
    public GameItem Item;
    public string ItemName;
    public Vector3 Position;
    public int amount;

    public DroppedItems(SerializableGuid id, GameItem item, Vector3 Position, int amount)
    {
        this.id = id;
        this.Item = item;
        this.ItemName = item.ItemTypeID;
        this.Position = Position;
        this.amount = amount;
    }
}


public class OnGroundItemManager : MonoBehaviour, IBind<DroppedItemSaveData>
{
    public static OnGroundItemManager Instance;
    [SerializeField] private GameObject droppedItemPrefab;
    [field: SerializeField] private DroppedItemSaveData droppedItemSaveData = new DroppedItemSaveData();

    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

    private void Awake()
    {
        Instance = this;
    }
    public void AddToAlreadyCollectedWorldSpawnItems(SerializableGuid id)
    {
        droppedItemSaveData.collectedWorldSpawnItems.Add(id);
    }
    public bool AlreadyCollectedItem(SerializableGuid id)
    {
        return droppedItemSaveData.collectedWorldSpawnItems.Contains(id);
    }
    public bool ContainsItem(SerializableGuid id)
    {
        return droppedItemSaveData.Contains(id);
    }

    public void RegisterDroppedItem(SerializableGuid id,GameItem item, Vector3 position, int amount)
    {
        droppedItemSaveData.AddItem(id, item, position, amount);
    }

    public void DeReisterDroppedItem(SerializableGuid guid)
    {
        droppedItemSaveData.RemoveItem(guid);
    }

    public void DropItem(GameItem item, Vector3 position, int amount, bool value = true)
    {
        DroppedGameItem newitem = Instantiate(droppedItemPrefab, position, Quaternion.identity).GetComponent<DroppedGameItem>();
        if (newitem != null)
        {
            newitem.SpawnNewItem(item, amount, value);
        }
    }


    public void Bind(DroppedItemSaveData data)
    {
        droppedItemSaveData.droppedItems.Clear();
        foreach (DroppedItems item in data.droppedItems)
        {
            GameItem itemToDrop = item.Item;
            ItemData itemdata = DataBase.GetItem(item.ItemName);
                                //DataBaseManager.Instance.ItemDataBase.GetItem(item.ItemName);
            if(itemdata != null)
            {
                itemToDrop.SetItemData(itemdata);
                DroppedGameItem newitem = Instantiate(droppedItemPrefab, item.Position, Quaternion.identity).GetComponent<DroppedGameItem>();
                newitem.Init(itemToDrop, item.amount, true);
                newitem.SetId(item.id);
            }
            else
            {
                Debug.Log("Item Is Null");
            }
           
        }
        droppedItemSaveData = data;
        data.Id = Id;
    }
}
