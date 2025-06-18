using GameSystems.BuildingSystem;
using UnityEngine;

public class BuildingTree : BuildingBase
{
    [SerializeField] private SpriteRenderer visuals;

    public override bool Interact(InteractionAttempt interactor)
    {
        if (interactor.Intent == InteractionIntent.Harvest_Wood)
        {
            int damage = interactor.Slot.GameItem.GameItemData?.ItemAttackDamage ?? 0;
            DamageBuilding(damage);
            return true;
        }
        return false;
    }
   
    public override void InitializeBuilding(Buildable building)
    {
        base.InitializeBuilding(building);
        itemsToDropOnDestroy = building.BuildableData.ItemsToDropOnDestroy;
        UpdateVisuals();

    }
    public override void UpdateVisuals()
    {
        visuals.sprite = building.BuildableData.GrowthStage.GrowthStagesList[0].viusal;
        base.UpdateVisuals();
        
    }

    public override void OnInteractingEnd()
    {
        
    }

    protected override void HandleHighLighting()
    {
        EventBus<OnIteractableHovered>.Raise(new OnIteractableHovered
        {
            interactable = this,
            interactionPosition = transform.position,
        });
    }


    public override bool CanAcceptInteractionType(InteractionAttempt interactionAttempt)
    {
        if(interactionAttempt.Intent == InteractionIntent.Harvest_Wood) return true;
        
        return base.CanAcceptInteractionType(interactionAttempt);
    }

    public override OnToolTipRequested GetToolTip()
    {
       string name = building.BuildableData?.DisplayName?? "Tree";
        OnToolTipRequested tooltip = new OnToolTipRequested
        {
            toolTipHeader = name,
            intent = InteractionIntent.Harvest_Wood,
        };
        return tooltip;
    }

    public override void Damage(int damage, DamageType damageType)
    {
        if(damageType == DamageType.Harvest_Wood)
        {
            DamageBuilding(damage);
        }
    }
}
