using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoulTraitsForReference", menuName = "Scriptable Objects/SoulTraitsForReference")]
public class SoulTraitsForReference : ScriptableObjectWithId
{
    [field: SerializeField] public bool HasMultipleTraits { get; private set; }
    [field: SerializeField] public List<StatStages<List<AlignmentAdjustment>>> StatStages { get; private set; }
    [field: SerializeField] public List<ThingTag> ThingTags { get; private set; }
    

    public string GetNameAtIndex(int index)
    {
        return StatStages[Mathf.Clamp(index, 0, StatStages.Count)].DisplayName;
    }
    public string GetDescriptionAtIndex(int index)
    {
        return StatStages[Mathf.Clamp(index, 0, StatStages.Count)].Description;
    }
    
   

    public SoulTrait GetRandomSoulTrait()
    {
        int rand = UnityEngine.Random.Range(0,StatStages.Count);
        string id = HasMultipleTraits? $"{Id}({rand})" : Id;
        return new SoulTrait(id, StatStages[rand].DisplayName, StatStages[rand].Value);
    }
    public SoulTrait GetSoulTraitAtIndex(int index)
    {
        index = Mathf.Clamp(index, 0, StatStages.Count);

        int numtoGet = HasMultipleTraits ? index : 0;

        string id = HasMultipleTraits ? $"{Id}({index})" : Id;

        return new SoulTrait(id, StatStages[numtoGet].DisplayName, StatStages[numtoGet].Value);
    }

    public List<SoulTrait> GetAllSoulTraits()
    {
        List<SoulTrait> list = new List<SoulTrait>();
        int index = 0;
        foreach (var t in StatStages)
        {
            string id = $"{Id}({index})";
            list.Add(new SoulTrait(id, t.DisplayName, t.Value));
            index++;
        }
        return list;
    }
}

[Serializable]
public class SoulTrait
{
    [field: SerializeField] public string Id { get; private set; }
    [field: SerializeField] public string Name { get; private set; }

    [field: SerializeField] public List<AlignmentAdjustment> AlignmentAdjustments { get; private set; }

    public SoulTrait(string id, string name, List<AlignmentAdjustment> alignmentAdjustments)
    {
        Id = id;
        Name = name;
        this.AlignmentAdjustments = alignmentAdjustments;
    }
}
