using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SoulBackStoryPreset", menuName = "Scriptable Objects/SoulBackStoryPreset")]
public class SoulBackStoryPreset : BaseSoulBaseStoryPreset
{
    [field: SerializeField] public string FirstName { get; private set; }
    [field: SerializeField] public string NickName { get; private set; }
    [field: SerializeField] public string LastName { get; private set; }

    [field: SerializeField] public SoulBackStoryPart ChildHoodPart { get; private set; }
    [field: SerializeField] public SoulBackStoryPart AdultHoodPart { get; private set; }
    [field: SerializeField] public SoulBackStoryPart DeathCausePart { get; private set; }
    [field: SerializeField] public SoulGender Gender { get; private set; }

    [field: SerializeField] public List<ThingAmount<SoulTraitsForReference>> AdditionalTraits { get; private set; }


    public override SoulBackStory GenerateBackStoryFromPreset()
    {
        
        string fullName = $"{FirstName} {NickName} {LastName}";
        return new SoulBackStory(fullName, ChildHoodPart, AdultHoodPart, DeathCausePart, Gender);
        
    }

    public override List<SoulTrait> GenerateSoulTraitFromPreset(int number)
    {
        List<ThingAmount<SoulTraitsForReference>> list = AdditionalTraits.OrderBy(i => Guid.NewGuid()).ToList();
        List<SoulTrait> traits = new List<SoulTrait>();
        int max = Mathf.Min(number, AdditionalTraits.Count);
        for (int i = 0; i < max; i++)
        {
            if (list[i].amount == -1) traits.Add(list[i].Thing.GetRandomSoulTrait());
            else traits.Add(list[i].Thing.GetSoulTraitAtIndex(list[i].amount));
        }
        return traits;
    }
}
