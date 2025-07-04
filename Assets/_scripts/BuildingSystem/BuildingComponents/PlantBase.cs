using Callendar;
using GameSystems.BuildingSystem;
using GameSystems.Inventory;
using System;
using UnityEngine;
using UnityEngine.Events;

[Obsolete]
[RequireComponent(typeof(BuildingSerializableGUID))]
public abstract class PlantBase : BuildingBase
{

    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected DateTimeStamp plantedTime;

    [SerializeField] protected int growthStageIndex = 0;

    [SerializeField] protected GameItem harvestItem;

    protected GrowthStage currentGrowthstage => building.BuildableData.GrowthStage.GetGrowthStageAtIndex(growthStageIndex);
    public GrowthStage CurrentGrowthstage => currentGrowthstage;

    protected int harvestableGrowthStage;
    protected int daysThisStage;



    public virtual void Harvest()
    {
        DropItems(harvestItem, 1);
        RemoveBuilding();
    }


  
}
