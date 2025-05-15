using Callendar;
using GameSystems.BuildingSystem;
using GameSystems.Inventory;
using UnityEngine;



public class GardenPlantObject : BuildingBase
{
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected DateTimeStamp plantedTime;

    [SerializeField] protected int growthStageIndex = 0;

    [SerializeField] protected GameItem harvestItem;

    protected GrowthStage currentGrowthstage => building.BuildableData.GrowthStage.GetGrowthStageAtIndex(growthStageIndex);
    public GrowthStage CurrentGrowthstage => currentGrowthstage;

    protected int harvestableGrowthStage;
    protected int daysThisStage;



    public void Harvest()
    {
        DropItems(harvestItem, 1);
        RemoveBuilding();
    }

    private void Start()
    {
        TimeManager.OnNewDay += GameTick;
    }

    private void OnDisable()
    {
        TimeManager.OnNewDay -= GameTick;
    }

    [ContextMenu("Debug Grow One Day")]
    private void IncrimentStage()
    {
        daysThisStage++;
        if (daysThisStage >= currentGrowthstage.daysForNextStage)
        {
            growthStageIndex++;
            daysThisStage = 0;
            UpdateVisuals();
        }
    }
    public void GameTick(DateTimeStamp stamp)
    {
        IncrimentStage();
    }


    public override void InitializeBuilding(Buildable plantType)
    {
        base.InitializeBuilding(plantType);
        plantedTime = TimeManager.Instance.CurrentGameTime;
        harvestItem = plantType.BuildableData.GrowthStage.harvestableGameItemToDrop;
        harvestableGrowthStage = plantType.BuildableData.GrowthStage.HarvestableGrowDays;
        UpdateVisuals();
     
    }


    public override void UpdateVisuals()
    {
        if(CurrentGrowthstage.viusal != null)
        {
            spriteRenderer.sprite = CurrentGrowthstage.viusal;
        }
        
        base.UpdateVisuals();
    }
    public override bool Interact(InteractionAttempt interactionAttempt)
    {
        int damage = interactionAttempt.Item.GameItemData?.ItemAttackDamage ?? 0;
       
        Debug.Log("interact");
        if(interactionAttempt.Intents.Contains(InteractionIntent.Harvest_Plants))
        {
            if (growthStageIndex >= harvestableGrowthStage)
            {
                Harvest();
                return true;
            }
            else
            {
                DamageBuilding(damage*2);
                return true;
            }
        }
        else if (interactionAttempt.Intents.Contains(InteractionIntent.Interact))
        {
            if (growthStageIndex >= harvestableGrowthStage)
            {
                Harvest();
                return true;
            }
        }
        else if (interactionAttempt.Intents.Contains(InteractionIntent.Attack))
        {
            DamageBuilding(damage);
        }
        return false;
    }

    public override void OnInteractingEnd()
    {
        
    }
}
