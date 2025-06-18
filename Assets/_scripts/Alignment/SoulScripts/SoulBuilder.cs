using System.Collections.Generic;
using UnityEngine;

public static class SoulBuilder
{

    public static SoulData Default
    {
        get
        {
            return GenerateSoul(SoulBackStory.Default, new List<SoulTrait>(), 3);
        }
       
    }

    public static SoulData GenerateRandomSoulFromPreset(BaseSoulBaseStoryPreset preset, int maxTraitsToGenerate)
    {
        SoulBackStory story = preset.GenerateBackStoryFromPreset();
        List<SoulTrait> list = preset.GenerateSoulTraitFromPreset(maxTraitsToGenerate);
        return GenerateSoul(story, list, maxTraitsToGenerate);
    }


    public static SoulData GenerateSoul(SoulBackStory backstory, List<SoulTrait> PotentialAditionalTraits, int maxTraitsToGenerate)
    {
        
        List<AlignmentAdjustment> listmaster = GetAllPossibleAlignmentAdjustmentsFromBackstory(backstory);
        List<SoulTrait> SoulTraitsFromBackstory = GetAllForcedTraitsFromBackStory(backstory);
        List<SoulTrait> FinalSoulTraitList = new List<SoulTrait>();
        if(SoulTraitsFromBackstory.Count > maxTraitsToGenerate)
        {
            for(int i = 0; i < maxTraitsToGenerate; ++i)
            {
                FinalSoulTraitList.Add(SoulTraitsFromBackstory[i]);
            }
        }
        
        else
        {
            int maxPossibleTraits = Mathf.Min(SoulTraitsFromBackstory.Count + PotentialAditionalTraits.Count, maxTraitsToGenerate);
            int random = UnityEngine.Random.Range(SoulTraitsFromBackstory.Count, maxPossibleTraits);

            List<SoulTrait> joinList = new List<SoulTrait>(SoulTraitsFromBackstory);
            joinList.AddRange(PotentialAditionalTraits);

            for (int i = 0; i < random; ++i)
            {
                FinalSoulTraitList.Add(joinList[i]);
            }

        }

        listmaster.AddRange(GetAllPossibleAlignmentAdjustmentsFromTraitList(FinalSoulTraitList));

        Dictionary<AlignmentType,int> offset = new Dictionary<AlignmentType,int>();
        Dictionary<AlignmentType,int> factor = new Dictionary<AlignmentType,int>();

        foreach (var t in listmaster)
        {
            if(t.applicationType == SoulMathType.StatOffsetPercentage)
            {
                if (offset.ContainsKey(t.TypeToModify)) offset[t.TypeToModify] += t.amountToModify;
                else offset.Add(t.TypeToModify, t.amountToModify);

            }
            else if(t.applicationType == SoulMathType.StatFactorPercentage)
            {
                if (factor.ContainsKey(t.TypeToModify)) factor[t.TypeToModify] += t.amountToModify;
                else factor.Add(t.TypeToModify, t.amountToModify);

            }
        }

        Alignment alignment = new Alignment();

        foreach (var t in offset)
        {
            alignment.EffectRangeTowards(t.Value, t.Key);
           
        }

        SoulData data = new SoulData(backstory, alignment, FinalSoulTraitList, factor);
        return data;

    }

    private static List<SoulTrait> GetAllForcedTraitsFromBackStory(SoulBackStory backstory)
    {
        List<SoulTrait> PotentialSoulTraits = new List<SoulTrait>();
        GetAllForcedTraitsFromBackStoryPart(backstory.ChildHoodPart, PotentialSoulTraits);
        GetAllForcedTraitsFromBackStoryPart(backstory.AdultHoodPart, PotentialSoulTraits);
        GetAllForcedTraitsFromBackStoryPart(backstory.DeathCausePart, PotentialSoulTraits);
        return PotentialSoulTraits;
    }

    private static void GetAllForcedTraitsFromBackStoryPart(SoulBackStoryPart backstory, List<SoulTrait> PotentialSoulTraits)
    {
        foreach (var t in backstory.GetAllSoulTraits())
        {
            if (!PotentialSoulTraits.Contains(t)) PotentialSoulTraits.Add(t);
        }
    }

    private static List<AlignmentAdjustment> GetAllPossibleAlignmentAdjustmentsFromTraitList(List<SoulTrait> traits)
    {
        List<AlignmentAdjustment> list = new List<AlignmentAdjustment>();
        foreach (SoulTrait trait in traits)
        {
            list.AddRange(trait.AlignmentAdjustments);
        }
        return list;
    }

    private static List<AlignmentAdjustment> GetAllPossibleAlignmentAdjustmentsFromBackstory(SoulBackStory backstory)
    {
        List<AlignmentAdjustment> listmaster = new List<AlignmentAdjustment>();
        listmaster.AddRange(backstory.ChildHoodPart.ConnectedAlignments);
        listmaster.AddRange(backstory.AdultHoodPart.ConnectedAlignments);
        listmaster.AddRange(backstory.DeathCausePart.ConnectedAlignments);
        return listmaster;
    }
}
