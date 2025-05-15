using GameSystems.BuildingSystem;
using GameSystems.Inventory;
using GameSystems.SaveLoad;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BuildingSerializableGUID))]

public abstract class BuildingBase : MonoBehaviour, IInteractable
{
    
    [SerializeField] protected List<ItemData> itemsToDropOnDestroy;

    [SerializeField] protected GameObject constructedVisuals;

    [SerializeField] protected Buildable building;
    public Buildable Building => building;

    protected WorldSpaceInventory buildingInventory;
    public WorldSpaceInventory BuildingInventory => buildingInventory;
    public BuildingHealth health;

    public SerializableGuid Id { get ; set ; }

    public BuildableTiles BuildingType => building.BuildableData;

    public UnityAction OnInteraction { get; set; }
    public UnityAction OnInteractionEnd { get; set; }

    protected virtual void Awake()
    {
        buildingInventory = GetComponentInChildren<WorldSpaceInventory>();
        health = GetComponentInChildren<BuildingHealth>();
        if (health != null)
        {
            health.OnBuildingDeath += RemoveBuilding;
        }
    }


    public virtual void InitializeBuilding(Buildable building)
    {
        if(building == null||building.BuildableData == null) return;
        this.building = building;

        Id = GetComponent<BuildingSerializableGUID>().Id;
        itemsToDropOnDestroy = this.building.BuildableData?.ItemsToDropOnDestroy;
        health?.InitializeHealth(this.building.BuildableData.BuildingHealth);

    }

    public virtual void DamageBuilding(int damage)
    {
        health?.TryDamage(damage);
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
        EventBus<OnBuildingRemovalRequested>.Raise(new OnBuildingRemovalRequested
        {
            position = transform.position
        });
        
    }


    

}
