
using GameSystems.BuildingSystem;
using GameSystems.Inventory;
using GameSystems.ShopSystem;
using UnityEngine;

public interface IEvent { }


public struct OnPlayerEquipedItemChanged : IEvent {
    public InventorySlot Slot;
    public GameItem Item;
}

public struct OnPlayerInventoryRequested: IEvent { 
    public GameItem Item; 
}  


public struct OnBuildingRemovalRequested : IEvent
{
    public Vector3 position;
}

public struct ClearSpawnForagables : IEvent
{

}
public struct OnDropItemAtPositionRequested : IEvent
{
    public GameItem item;
    public Vector3 position;
    public int amount;
    public bool pickupable;
    public OnDropItemAtPositionRequested(GameItem item, Vector3 position, int amount, bool immediatelyPickUpable = false)
    {
        this.item = item;
        this.position = position;
        this.amount = amount;
        this.pickupable = immediatelyPickUpable;
    }
}

public struct OnShopScreenRequested: IEvent
{
    public ShopSystem Shop;
}
public struct OnDynamicInventoryRequested : IEvent
{
    public InventorySystem inventorySystem;
    public int offset;
}

public struct OnGameStart : IEvent { }
public struct OnTrySpawnFolliage : IEvent
{
    public Vector3 spawnPosition;
    public BuildableTiles BuildableTiles;
    public BuildingType BuildingType;
}