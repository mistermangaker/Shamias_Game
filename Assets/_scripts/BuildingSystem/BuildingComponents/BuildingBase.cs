using GameSystems.BuildingSystem;
using GameSystems.Inventory;
using GameSystems.SaveLoad;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BuildingSerializableGUID))]

public abstract class BuildingBase : MonoBehaviour, IInteractable, IToolTip, IHighLightable
{
    protected HighlightableSprite HighlightableSprite;

    [SerializeField] protected List<ItemData> itemsToDropOnDestroy;

    [SerializeField] protected GameObject constructedVisuals;

    [SerializeField] protected Buildable building;
    public Buildable Building => building;

    protected WorldSpaceInventory buildingInventory;
    public WorldSpaceInventory BuildingInventory => buildingInventory;
    public DamageableHealth health;

    public SerializableGuid Id { get ; set ; }

    public BuildableTiles BuildingType => building.BuildableData;

    public UnityAction OnInteraction { get; set; }
    public UnityAction OnInteractionEnd { get; set; }

    protected virtual void Awake()
    {
        buildingInventory = GetComponentInChildren<WorldSpaceInventory>();
        health = GetComponentInChildren<DamageableHealth>();
        if (health != null)
        {
            health.OnBuildingDeath += RemoveBuilding;
        }
        HighlightableSprite = GetComponentInChildren<HighlightableSprite>();
    }

    protected virtual void OnEnable()
    {
        if(HighlightableSprite != null)
        {
            HighlightableSprite.OnHoverOver += HandleHighLighting;
            HighlightableSprite.OnHoverOut += HandleUnHighLighting;
        }
    }

    protected virtual void OnDisable()
    {
        if (HighlightableSprite != null)
        {
            HighlightableSprite.OnHoverOver -= HandleHighLighting;
            HighlightableSprite.OnHoverOut -= HandleUnHighLighting;
        }
    }

    protected virtual void HandleHighLighting() { }
    protected virtual void HandleUnHighLighting() 
    {
        EventBus<OnIteractableUnHovered>.Raise(new OnIteractableUnHovered());
    }

    public virtual void InitializeBuilding(Buildable building)
    {
        if(building == null||building.BuildableData == null) return;
        this.building = building;

        Id = GetComponent<BuildingSerializableGUID>().Id;
        itemsToDropOnDestroy = this.building.BuildableData?.ItemsToDropOnDestroy;
        health?.InitializeHealth(this.building.BuildableData.BuildingHealth, this.building.BuildableData.DamageFactors);
        UpdateVisuals();
    }

    public virtual void DamageBuilding(int damage)
    {
        health?.Damage(damage);
    }
    public abstract bool Interact(InteractionAttempt interactor);
  

    public virtual void UpdateVisuals()
    {
        HighlightableSprite?.UpdateCollider();
    }

    public abstract void OnInteractingEnd();
  
    public virtual void DropItems(GameItem item, int amount)
    {
        if(gameObject == null) return;
        EventBus<OnDropItemAtPositionRequested>.Raise(new OnDropItemAtPositionRequested
        {
            amount = amount,
            item = item,
            position = transform.position,
            pickupable = true
        });
       
    }

    public virtual void RemoveBuilding()
    {
        foreach(ItemData item in itemsToDropOnDestroy)
        {
            GameItem gameitem = new GameItem.Builder().Build(item);
            DropItems(gameitem,1);
        }
       HandleUnHighLighting();
        EventBus<OnBuildingRemovalRequested>.Raise(new OnBuildingRemovalRequested
        {
            position = transform.position
        });
        
    }

    public virtual bool CanAcceptInteractionType(InteractionAttempt interactionAttempt)
    {
        if (interactionAttempt.Intent == InteractionIntent.Interact) return true;
        return false;
    }

    public void Highlight()
    {
        HighlightableSprite.Hover();
    }

    public void UnHighLight()
    {
        HighlightableSprite.UnHover();
    }

    public abstract OnToolTipRequested GetToolTip();

    public abstract void Damage(int damage, DamageType damageType);
    
}
