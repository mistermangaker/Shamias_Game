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
    [field: SerializeField] public int DaysBetweenStages { get; private set; }
    [field: SerializeField]public ItemData HarvestableItemToDrop { get; private set; }
    public int AmountToDrop;

    public GameItem HarvestableGameItemToDrop
    {
        get
        {
            return GameItem.DefaultItem(HarvestableItemToDrop);
        }
    }

    public GrowthStage GetGrowthStageAtIndex(int index)
    {
        if (GrowthStagesList.Count > index) return GrowthStagesList[index];
        else return GrowthStagesList[GrowthStagesList.Count-1];
    }

}


[Serializable()]
public struct GrowthStage
{
    public string Debug_Id;
    public Sprite viusal;
    [field: SerializeField] public ItemData harvestABleItemToDropThisStage;
    public bool CanBeHarvestedByHand;
    public int AmountToDrop;
}
