using GameSystems.BuildingSystem;
using GameSystems.Inventory;
using UnityEngine;

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager Instance;

    [field:SerializeField] public BuildablesDataBase BuildablesDataBase {  get; private set; }
    [field: SerializeField] public ItemDataBase ItemDataBase { get; private set; }

    public BuildableTiles GetBuildableByID(string id)
    {
        return BuildablesDataBase.GetBuildable(id);
    }
    public ItemData GetItemDataByID(string id)
    {
        return ItemDataBase.GetItem(id);
    }
    public void Awake()
    {
        Instance = this;
    }

    [ContextMenu("Set Ids")]
    public void SetIds()
    {
        BuildablesDataBase.SetItemIDs();
        ItemDataBase.SetItemIDs();
    }
}
