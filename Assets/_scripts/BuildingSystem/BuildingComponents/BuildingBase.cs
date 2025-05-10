using GameSystems.BuildingSystem;
using GameSystems.Inventory;
using GameSystems.SaveLoad;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BuildingSaveData))]

public abstract class BuildingBase : MonoBehaviour, IInteractable
{
    
    [SerializeField] protected List<ItemData> itemsToDropOnDestroy;

    [SerializeField] protected GameObject constructedVisuals;

    [SerializeField] protected Buildable building;
    public Buildable Building => building;

    protected WorldSpaceInventory buildingInventory;
    public WorldSpaceInventory BuildingInventory => buildingInventory;

    public SerializableGuid Id { get ; set ; }

    public BuildableTiles BuildingType => building.BuildableType;

    public UnityAction OnInteraction { get; set; }
    public UnityAction OnInteractionEnd { get; set; }

    protected virtual void Awake()
    {
        buildingInventory = GetComponentInChildren<WorldSpaceInventory>();
    }


    public virtual void InitializeBuilding(Buildable building)
    {
        this.building = building;

        Id = GetComponent<BuildingSaveData>().Id;

    }

    public abstract bool Interact(InteractionAttempt interactor);
  

    public virtual void UpdateVisuals()
    {
      
    }

    public abstract void OnInteractingEnd();
  
    public virtual void DropItems(GameItem item, int amount)
    {
        OnGroundItemManager.Instance?.DropItem(item, transform.position, amount);
    }

    public virtual void RemoveBuilding()
    {
        foreach(ItemData item in itemsToDropOnDestroy)
        {
            GameItem gameitem = new GameItem.Builder().Build(item);
            DropItems(gameitem,1);
        }
        ConstructionLayerManager.Instance.RemoveBuilding(transform.position);
    }


    

}
