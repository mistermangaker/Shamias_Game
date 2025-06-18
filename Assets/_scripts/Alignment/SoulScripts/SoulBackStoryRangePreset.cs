
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "SoulBackStoryRangePreset", menuName = "Scriptable Objects/SoulBackStoryRangePreset")]
public class SoulBackStoryRangePreset : BaseSoulBaseStoryPreset
{
    [Header("Naming ")]
    [field: SerializeField] public List<string> PotentialFullNames { get; private set; }
    [SerializeField, Range(0f, 1f)] private float PrioritizeFullNamesPercentage;
    [field: SerializeField] public List<string> PotentialFirstNames { get; private set; }
    [field: SerializeField] public List<string> PotentialNickNames{ get; private set; }

    [SerializeField, Range(0f,1f)] private float emptyNickNamePercentageChance;
    public float EmptyNickNamePercentageChance => emptyNickNamePercentageChance;
    [field: SerializeField] public List<string> PotentialLastNames { get; private set; }



    [Header("BackStory Parts Generation Settings")]
    [field: SerializeField] public List<SoulBackStoryPart> PotentialChildHoodParts { get; private set; }
    [field: SerializeField] public List<SoulBackStoryPart> PotentialAdultHoodParts { get; private set; }
    [field: SerializeField] public List<SoulBackStoryPart> PotentialDeathCauseParts { get; private set; }
    [field: SerializeField] public SoulGenderPresetSettings PotentialGenders { get; private set; }
    [SerializeField, Range(0f, 1f)] private float FemaleLikelyHoodPercent;

    [Header("Thing Tag Generation Settings")]
    [field: SerializeField] public List<ThingTag> ThingTags { get; private set; }
    [field: SerializeField] public bool UseThingTagsForTraits { get; private set; }
    [field: SerializeField] public List<ThingAmount<SoulTraitsForReference>> AdditionalTraits { get; private set; }
    [SerializeField, Range(0f, 1f)] private float AdditionalTraitsBiasInRandomGenerationBias;
    
    [field: SerializeField] public bool UseThingTagsForBackStories { get; private set; }
    [SerializeField, Range(0f, 1f)] private float BackGroundPresetBiasInRandomGeneration;




    public override SoulBackStory GenerateBackStoryFromPreset()
    {

        bool useFullName = Random.value > PrioritizeFullNamesPercentage;
        bool useEmptyNickName = Random.value > emptyNickNamePercentageChance;
        string nicName = useEmptyNickName ? "" : PotentialNickNames[Random.Range(0, PotentialNickNames.Count)];
        string fullName = "";
        if (useFullName)
        {
            fullName = PotentialFullNames[Random.Range(0, PotentialFullNames.Count)];
        }
        else
        {
            string firstName = PotentialFirstNames[Random.Range(0, PotentialFirstNames.Count)];
            string lastName = PotentialLastNames[Random.Range(0, PotentialLastNames.Count)];
            fullName = $"{firstName} {nicName} {lastName} ";
        }
        SoulBackStoryPart childhood = null;
        SoulBackStoryPart adulthood = null; 
        SoulBackStoryPart deathcause = null; 
        if (UseThingTagsForBackStories)
        {
            if (BackGroundPresetBiasInRandomGeneration > Random.value) childhood = DataBase.GetRandomSoulBackStoryPartFromDataBaseByLifeTimeTag(SoulBackStoryLifeTimeTag.ChildHood, ThingTags[Random.Range(0, ThingTags.Count)]);
            else childhood = PotentialChildHoodParts[Random.Range(0, PotentialChildHoodParts.Count)];
            if (BackGroundPresetBiasInRandomGeneration > Random.value) adulthood = DataBase.GetRandomSoulBackStoryPartFromDataBaseByLifeTimeTag(SoulBackStoryLifeTimeTag.AdultHood, ThingTags[Random.Range(0, ThingTags.Count)]);
            else adulthood = PotentialAdultHoodParts[Random.Range(0, PotentialAdultHoodParts.Count)];
            if (BackGroundPresetBiasInRandomGeneration > Random.value) deathcause = DataBase.GetRandomSoulBackStoryPartFromDataBaseByLifeTimeTag(SoulBackStoryLifeTimeTag.DeathCause, ThingTags[Random.Range(0, ThingTags.Count)]);
            else deathcause = PotentialDeathCauseParts[Random.Range(0, PotentialDeathCauseParts.Count)];
        }
        else
        {
           childhood = PotentialChildHoodParts[Random.Range(0, PotentialChildHoodParts.Count)];
             adulthood = PotentialAdultHoodParts[Random.Range(0, PotentialAdultHoodParts.Count)];
             deathcause = PotentialDeathCauseParts[Random.Range(0, PotentialDeathCauseParts.Count)];
        }
        
        SoulGender gender;
        if(PotentialGenders == SoulGenderPresetSettings.Male)
        {
            gender = SoulGender.Male;
               
        }
        else if (PotentialGenders == SoulGenderPresetSettings.Female)
        {
            gender= SoulGender.Female;
        }
        else
        {
            bool isFemale = Random.value > PrioritizeFullNamesPercentage;
            gender = isFemale ? SoulGender.Female : SoulGender.Male;
        }

        return new SoulBackStory(fullName, childhood, adulthood, deathcause, gender);
        
    }

    public override List<SoulTrait> GenerateSoulTraitFromPreset(int number)
    {
        List<ThingAmount<SoulTraitsForReference>> list = AdditionalTraits.OrderBy(i => Guid.NewGuid()).ToList();
        List<SoulTraitsForReference> traitsList = new List<SoulTraitsForReference>();
        if (UseThingTagsForTraits)
        {
            foreach (var tags in ThingTags)
            {
                traitsList.AddRange(DataBase.GetAllSoulTraitsFromTag(tags));
            }
        }
        
        List<SoulTrait> traits = new List<SoulTrait>();
        int max = Mathf.Min(number, UseThingTagsForTraits? AdditionalTraits.Count  + traitsList.Count : AdditionalTraits.Count);
        for (int i = 0; i < max; i++)
        {
            if(traitsList.Count > 0)
            {
                if (AdditionalTraitsBiasInRandomGenerationBias < Random.value)
                {
                    SoulTrait newTrait = traitsList[Random.Range(0, traitsList.Count)].GetRandomSoulTrait();
                    if(!traits.Contains(newTrait)) traits.Add(newTrait);
                    else
                    {
                        if (list[i].amount == -1) traits.Add(list[i].Thing.GetRandomSoulTrait());
                        else traits.Add(list[i].Thing.GetSoulTraitAtIndex(list[i].amount));
                    }
                }
                else
                {
                    if (list[i].amount == -1) traits.Add(list[i].Thing.GetRandomSoulTrait());
                    else traits.Add(list[i].Thing.GetSoulTraitAtIndex(list[i].amount));
                }
            }
            else
            {
                if (list[i].amount == -1) traits.Add(list[i].Thing.GetRandomSoulTrait());
                else traits.Add(list[i].Thing.GetSoulTraitAtIndex(list[i].amount));
            }
            
        }
        return traits;
    }
}

public enum SoulGenderPresetSettings
{
    Male,
    Female,
    Both
}
