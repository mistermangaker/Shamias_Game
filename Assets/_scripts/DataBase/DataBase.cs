using GameSystems.BuildingSystem;
using GameSystems.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Searcher.Searcher.AnalyticsEvent;
using Random = UnityEngine.Random;

public static class DataBase
{
    private static Dictionary<SoulBackStoryLifeTimeTag, List<SoulBackStoryPart>> BackStoryParts = new Dictionary<SoulBackStoryLifeTimeTag, List<SoulBackStoryPart>>();
   
    private static Dictionary<string,SoulBackStoryPart> BackStoryFromId = new Dictionary<string, SoulBackStoryPart>();

    private static Dictionary<string, SoulTraitsForReference> Traits = new Dictionary<string, SoulTraitsForReference>();
    private static Dictionary<string, SoulTrait> ingameTraits = new Dictionary<string, SoulTrait>();
    private static Dictionary<ThingTag, HashSet<SoulTraitsForReference>> AllTraitsFromThingTags = new Dictionary<ThingTag, HashSet<SoulTraitsForReference>>();


    private static Dictionary<ThingTag, HashSet<SoulBackStoryPart>> AllBackStoryFromThingTag = new Dictionary<ThingTag, HashSet<SoulBackStoryPart>>();
    private static Dictionary<ThingTag, HashSet<SoulBackStoryPart>> childhoodbackStoryFromThingTag = new Dictionary<ThingTag, HashSet<SoulBackStoryPart>>();
    private static Dictionary<ThingTag, HashSet<SoulBackStoryPart>> adulthoodbackStoryFromThingTag = new Dictionary<ThingTag, HashSet<SoulBackStoryPart>>();
    private static Dictionary<ThingTag, HashSet<SoulBackStoryPart>> deathcausebackStoryFromThingTag = new Dictionary<ThingTag, HashSet<SoulBackStoryPart>>();



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

        var allBackstories = Resources.LoadAll<SoulBackStoryPart>("SoulBackStories").ToList();

        foreach(var backstory in allBackstories)
        {
            if (BackStoryFromId.ContainsKey(backstory.Id)) 
            {
                Debug.LogWarning($"ScriptableObject of Name \"{backstory.name}\" has an Id \'{backstory.Id}\' that is already in the database. please fix this");
                continue;
            }
            else
            {
                BackStoryFromId.Add(backstory.Id, backstory);
            }
                
        }
        List<SoulBackStoryPart> childHoodBackstories = allBackstories.FindAll(i => i.LifeStageTag == SoulBackStoryLifeTimeTag.ChildHood).ToList();
        List < SoulBackStoryPart > AdultHoodHoodBackstories = allBackstories.FindAll(i => i.LifeStageTag == SoulBackStoryLifeTimeTag.AdultHood).ToList();
        List < SoulBackStoryPart > DeathCausesBackstories = allBackstories.FindAll(i => i.LifeStageTag == SoulBackStoryLifeTimeTag.DeathCause).ToList();
        //Debug.Log($"counts childhood: {childHoodBackstories.Count}  adulthood: {AdultHoodHoodBackstories.Count}  deathcauses: {DeathCausesBackstories.Count}");
        BackStoryParts.Add(SoulBackStoryLifeTimeTag.ChildHood, childHoodBackstories);
        BackStoryParts.Add(SoulBackStoryLifeTimeTag.AdultHood, AdultHoodHoodBackstories);
        BackStoryParts.Add(SoulBackStoryLifeTimeTag.DeathCause, DeathCausesBackstories);


        var allThingTags = Resources.LoadAll<ThingTag>("ThingTags").ToList();

        foreach (var thingTag in allThingTags)
        {
            AllBackStoryFromThingTag.Add(thingTag, new HashSet<SoulBackStoryPart>());
            childhoodbackStoryFromThingTag.Add(thingTag, new HashSet<SoulBackStoryPart>());
            adulthoodbackStoryFromThingTag.Add(thingTag, new HashSet<SoulBackStoryPart>());
            deathcausebackStoryFromThingTag.Add(thingTag, new HashSet<SoulBackStoryPart>());
            AllTraitsFromThingTags.Add(thingTag, new HashSet<SoulTraitsForReference>());

        }
        foreach(var backstory in allBackstories)
        {
            Dictionary<ThingTag, HashSet<SoulBackStoryPart>> thingToAddTo;
            if(backstory.LifeStageTag == SoulBackStoryLifeTimeTag.ChildHood)
            {
                thingToAddTo = childhoodbackStoryFromThingTag;
            }
            else if (backstory.LifeStageTag == SoulBackStoryLifeTimeTag.AdultHood)
            {
                thingToAddTo = adulthoodbackStoryFromThingTag;
            }
            else
            {
                thingToAddTo = deathcausebackStoryFromThingTag;
            }

            foreach (var tag in backstory.ThingTags)
            {
                if (!AllBackStoryFromThingTag[tag].Contains(backstory)) AllBackStoryFromThingTag[tag].Add(backstory);
                if (!thingToAddTo[tag].Contains(backstory)) AllBackStoryFromThingTag[tag].Add(backstory);
            }
        }

        var allTraits = Resources.LoadAll<SoulTraitsForReference>("Resources").ToList();
        foreach (var t in allTraits)
        {
            Traits.Add(t.Id, t);
        }
        foreach(var soultrait in allTraits)
        {
            List<SoulTrait> traits = soultrait.GetAllSoulTraits();
            foreach (var IngameSoulTrait in traits)
            {
                ingameTraits.Add(IngameSoulTrait.Id, IngameSoulTrait);
            }
            foreach (var tag in soultrait.ThingTags)
            {
                if (!AllTraitsFromThingTags[tag].Contains(soultrait)) AllTraitsFromThingTags[tag].Add(soultrait);
            }
        }

    }

    public static SoulTraitsForReference GetRandomSoulTraitsFromTag(ThingTag tag)
    {
        List<SoulTraitsForReference> temp = GetAllSoulTraitsFromTag(tag);
        SoulTraitsForReference part = temp[Random.Range(0, temp.Count)];
        return part;
    }
   
    public static List<SoulTraitsForReference> GetAllSoulTraitsFromTag(ThingTag tag)
    {
        return AllTraitsFromThingTags[tag].ToList();
    }

    public static string GetDefaultBackStoryIdFromLifeTimeTag(SoulBackStoryLifeTimeTag tag)
    {
        List<SoulBackStoryPart> temp = BackStoryParts[tag];
        SoulBackStoryPart part = temp.FirstOrDefault();
        return part != null ? part.Id : "";
    }

    public static SoulBackStoryPart GetRandomSoulBackStoryPartFromDataBaseByLifeTimeTag(SoulBackStoryLifeTimeTag tag)
    {
        List<SoulBackStoryPart> temp = BackStoryParts[tag];
        SoulBackStoryPart part  = temp[Random.Range(0, temp.Count)];
        return part;
    }
    public static SoulBackStoryPart GetRandomSoulBackStoryPartFromDataBaseByLifeTimeTag(SoulBackStoryLifeTimeTag tag, ThingTag tag2)
    {
        List<SoulBackStoryPart> temp = new List<SoulBackStoryPart>();
        foreach(SoulBackStoryPart story in BackStoryParts[tag])
        {
            if(story.LifeStageTag == tag) temp.Add(story);
        }

        SoulBackStoryPart part  = temp[Random.Range(0, temp.Count)];
        return part;
    }

    public static SoulBackStoryPart GetSoulBackStoryPartFromId(string id)
    {
        if(BackStoryFromId.ContainsKey(id)) { return BackStoryFromId[id]; } else { return null; }
    }

    //public static List<SoulTrait> GetAllSoulTraitsWithThingTag(ThingTag thingTag)
    //{

    //}


    public static bool BackStoryIdExists(string id)
    {
        return BackStoryFromId.TryGetValue(id, out var backStoryPart);
    }

    public enum AuthenticedID
    {
        authenticated,
        modified,
        failed
    }

    public static AuthenticedID HandleAuthenticatingBackStory(SoulBackStoryLifeTimeTag tagThisShouldBe , ref string id)
    {
        if(!BackStoryIdExists(id)) return AuthenticedID.failed;
        if(BackStoryFromId.TryGetValue(id, out var backStoryPart))
        { 
            if(backStoryPart.LifeStageTag != tagThisShouldBe)
            {

                id = GetDefaultBackStoryIdFromLifeTimeTag(tagThisShouldBe);

                return AuthenticedID.modified;
            }
            else
            {
                return AuthenticedID.authenticated;
            }
        }

        return AuthenticedID.failed;

    }
    public static void ClearDataBase()
    {
        AllBackStoryFromThingTag.Clear();
        AllBackStoryFromThingTag.Clear();
        childhoodbackStoryFromThingTag.Clear();
        adulthoodbackStoryFromThingTag.Clear();
        deathcausebackStoryFromThingTag.Clear();
        BackStoryParts.Clear();
        BackStoryFromId.Clear();
        _itemDatabase.Clear();
        _buildablesDataBase.Clear();
        Traits.Clear();
        ingameTraits.Clear();
        AllTraitsFromThingTags.Clear();
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
