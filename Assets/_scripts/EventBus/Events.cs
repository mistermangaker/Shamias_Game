
using GameSystems.BuildingSystem;
using GameSystems.Inventory;
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

public struct OnGameStart : IEvent { }
public struct OnTrySpawnFolliage : IEvent
{
    public Vector3 spawnPosition;
    public BuildableTiles BuildableTiles;
    public BuildingType BuildingType;
}