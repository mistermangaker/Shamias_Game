using GameSystems.BuildingSystem;
using UnityEngine;

public class BuildingTree : BuildingBase
{
    [SerializeField] private SpriteRenderer visuals;

    public override bool Interact(InteractionAttempt interactor)
    {
        if (interactor.Intents.Contains(InteractionIntent.Harvest_Wood))
        {
            int damage = interactor.Item.GameItemData?.ItemAttackDamage ?? 0;
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
        
    }

    public override void OnInteractingEnd()
    {
        
    }

   
}
