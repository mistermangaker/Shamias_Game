using GameSystems.BuildingSystem;
using GameSystems.Inventory;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Searcher.Searcher.AnalyticsEvent;

public static class DataBase
{
    //static readonly HashSet<ItemData> itemDatas = new HashSet<ItemData>();
    //static readonly HashSet<BuildableTiles> buildables = new HashSet<BuildableTiles>();
    private static List<ItemData> _itemDatabase = new List<ItemData>();
    private static List<BuildableTiles> _buildablesDataBase = new List<BuildableTiles>();

#if UNITY_EDITOR
    public static PlayModeStateChange PlayModeStateChange { get; set; }
    [InitializeOnLoadMethod]
    public static void InitializeEditor()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        PlayModeStateChange = state;
        if (PlayModeStateChange == PlayModeStateChange.ExitingEditMode)
        {
            ClearDataBase();
        }
    }
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitialIze()
    {
        _buildablesDataBase = new List<BuildableTiles>();

        var foundbuildings = Resources.LoadAll<BuildableTiles>("BuildableTilesDataBase").OrderBy(i => i.TileName).ToList();
        foreach (var tile in foundbuildings)
        {
           
            if (!_buildablesDataBase.Contains(tile)) _buildablesDataBase.Add(tile);
            else continue;
        }
        _buildablesDataBase.AddRange(foundbuildings);
        _itemDatabase = new List<ItemData>();

        var founditems = Resources.LoadAll<ItemData>("GameItemData").OrderBy(i => i.ItemId).ToList();
        foreach (var item in founditems)
        {
            
            if (!_itemDatabase.Contains(item)) _itemDatabase.Add(item);
            else continue;
        }
        _itemDatabase.AddRange(founditems);
    }

    public static void ClearDataBase()
    {
        _itemDatabase.Clear();
        _buildablesDataBase.Clear();
    }

    public static BuildableTiles GetBuildable(string itemID)
    {
        return _buildablesDataBase.Find(i => i.TileName == itemID);
    }
    public static ItemData GetItem(string itemID)
    {
        if (itemID == "null") return null;
        if (string.IsNullOrEmpty(itemID)) return null;
        return _itemDatabase.Find(i => i.ItemId == itemID);
    }
}
