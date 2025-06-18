using Callendar;
using GameSystems.BuildingSystem;
using GameSystems.Inventory;
using System.Collections.Generic;
using UnityEngine;



public class GardenPlantObject : BuildingBase
{
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected DateTimeStamp plantedTime;

    [SerializeField] protected int growthStageIndex = 0;

    [SerializeField] protected GameItem harvestItem;

    private DateTimeStamp nextGrowthDate;

    protected GrowthStage currentGrowthstage => building.BuildableData.GrowthStage.GetGrowthStageAtIndex(growthStageIndex);
    public GrowthStage CurrentGrowthstage => currentGrowthstage;

    public void RandomizeGrowthStage(int max = -1)
    {
        if(max > 0)
        {
            max = Mathf.Clamp(max,1, building.BuildableData.GrowthStage.GrowthStagesList.Count);
            growthStageIndex = Random.Range(0, max);
        }
        else
        {
            growthStageIndex = Random.Range(0, building.BuildableData.GrowthStage.GrowthStagesList.Count);
        }
        UpdateVisuals();
    }
    public bool IsHarvestableThisStage
    {
        get
        {
            return currentGrowthstage.harvestABleItemToDropThisStage != null;
        }
    }
    public bool IsHarvestableThisStageByHand
    {
        get
        {
            return currentGrowthstage.harvestABleItemToDropThisStage != null && currentGrowthstage.CanBeHarvestedByHand;
        }
    }

    public void Harvest()
    {
        if (currentGrowthstage.harvestABleItemToDropThisStage != null)
        {
            GameItem newItem = GameItem.DefaultItem(currentGrowthstage.harvestABleItemToDropThisStage);
           int amount= Mathf.Max(currentGrowthstage.AmountToDrop, 1);
            DropItems(newItem, amount);
        }
        DropItems(harvestItem, building.BuildableData.GrowthStage.AmountToDrop);
        RemoveBuilding();
    }

    private void Start()
    {
        TimeManager.OnNewDay += GameTick;
    }

    protected override void OnDisable()
    {
        TimeManager.OnNewDay -= GameTick;
    }

    [ContextMenu("Debug Grow One stage")]
    private void IncrimentStage()
    {
        growthStageIndex++;
        UpdateVisuals();
    }

    public void GameTick(DateTimeStamp stamp)
    {
        if(stamp >= nextGrowthDate)
        {
            DateTimeStamp currentDateTime = plantedTime;
            int incrimentTime = building.BuildableData.GrowthStage.DaysBetweenStages;
            int index = 0;
            foreach (var stage in building.BuildableData.GrowthStage.GrowthStagesList)
            {
                currentDateTime.AdvanceDay(incrimentTime);
                if (currentDateTime >= stamp) 
                {
                    growthStageIndex = index; 
                    nextGrowthDate = currentDateTime;
                    return;
                }
                else
                {
                    index++;
                }
            }
        }
        UpdateVisuals();
    }

    protected override void HandleHighLighting()
    {
        EventBus<OnIteractableHovered>.Raise(new OnIteractableHovered
        {
            name = building.BuildableData.DisplayName + currentGrowthstage.Debug_Id,
            interactable = this,
            interactionPosition = transform.position,
        });
    }
    

    public override void InitializeBuilding(Buildable plantType)
    {
        base.InitializeBuilding(plantType);
        plantedTime = TimeManager.Instance.CurrentGameTime;
        harvestItem = plantType.BuildableData.GrowthStage.HarvestableGameItemToDrop;
        nextGrowthDate = TimeManager.Instance.CurrentGameTime;
        nextGrowthDate.AdvanceDay(plantType.BuildableData.GrowthStage.DaysBetweenStages);
        
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
       // int damage = interactionAttempt.Slot.ItemData?.ItemAttackDamage ?? 0;
       
        
       if (interactionAttempt.Intent == InteractionIntent.Interact)
        {
            Debug.Log("interact");
            if (IsHarvestableThisStageByHand)
            {
                Harvest();
                return true;
            }
        }

        return false;
    }

    public override void OnInteractingEnd()
    {
        
    }

    public override bool CanAcceptInteractionType(InteractionAttempt interactionAttempt)
    {
        switch (interactionAttempt.Intent)
        {
            case InteractionIntent.Interact:
                return currentGrowthstage.harvestABleItemToDropThisStage != null;
            case InteractionIntent.Attack:
                return true;
            case InteractionIntent.Harvest_Plants:
                return true;
        }
        return false;
        
    }

    public override OnToolTipRequested GetToolTip()
    {
        List<InteractionIntent> interactions = new List<InteractionIntent>();
        InteractionIntent interactionIntent = InteractionIntent.None;
        string header = building.BuildableData.DisplayName;
        if (IsHarvestableThisStage)
        {
            interactions.Add(InteractionIntent.Harvest_Plants);
           
            if (IsHarvestableThisStageByHand)
            {
                interactionIntent = InteractionIntent.Interact;
                interactions.Add(InteractionIntent.Interact);
                header = $"Pick {building.BuildableData.DisplayName}";
            }
            else
            {
                interactionIntent = InteractionIntent.Harvest_Plants;
                header = $"Harvest {building.BuildableData.DisplayName}";
            }
        }
        return new OnToolTipRequested
        {
            intent = interactionIntent,
            toolTipHeader = header,
            possibleInteractions = interactions
        };
    }

    public override void Damage(int damage, DamageType damageType)
    {
        if(damageType == DamageType.Harvest_Plant)
        {
            if (IsHarvestableThisStage)
            {
                Harvest();
            }
            else
            {
                DamageBuilding(damage * 2);
            }
        }
      
    }
}
