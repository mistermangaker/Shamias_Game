using System.Collections.Generic;
using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using GameSystems.Inventory;

[CreateAssetMenu(fileName = "GrowthStages", menuName = "Building System/Plants/GrowthStages")]
public class GrowthStages : ScriptableObject
{
    [field: SerializeField] public string ID { get; private set; }
    [field: SerializeField] public List<GrowthStage> GrowthStagesList {  get; private set; }
    [field: SerializeField] public int HarvestableGrowDays { get; private set; }
    [field: SerializeField]public ItemData harvestableItemToDrop { get; private set; }

    public GameItem harvestableGameItemToDrop
    {
        get
        {
            return GameItem.DefaultItem(harvestableItemToDrop);
        }
    }

    public GrowthStage GetGrowthStageAtIndex(int index)
    {
        foreach (GrowthStage stage in GrowthStagesList)
        {
            if(stage.growthStageIndex == index) return stage;
        }
        if(GrowthStagesList.Count > index)
        {
            return GrowthStagesList[index];
        }
        return GrowthStagesList[0];
    }

}


[Serializable()]
public struct GrowthStage
{
    [field: SerializeField] public string GrowthStageName { get; private set; }
    [TextArea(4, 4)]
    public string GrowthStageDescriptionForViewing;

    public Sprite viusal;
    public GameObject newGameObject;
    public int daysForNextStage;
    public int growthStageIndex;


}
