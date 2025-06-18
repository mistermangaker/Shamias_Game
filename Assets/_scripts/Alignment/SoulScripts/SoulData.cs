
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SoulData
{
    [field: SerializeField] public SoulBackStory BackStory { get; private set; }

    [field: SerializeField] public Alignment SoulAlignment { get; private set; }

    [field: SerializeField] public List<SoulTrait> SoulTraits { get; private set; } = new List<SoulTrait>();

    [field: SerializeField] public Dictionary<AlignmentType,int> StatFactors { get; private set; } = new Dictionary<AlignmentType,int>();

    

    public SoulData(SoulBackStory BackStory)
    {
        this.BackStory = BackStory;
        this.SoulAlignment = BackStory.GetDefaultAlignmentFromBackStory();
        
    }
    public SoulData(SoulBackStory story,  Alignment alignment, List<SoulTrait> traits, Dictionary<AlignmentType,int> statFactors)
    {
        this.BackStory = story;
        this.SoulAlignment = alignment;
        this.SoulTraits = traits;
        this.StatFactors = statFactors;
    }


    private float GetStatFactorPercentageForAlignmentType(AlignmentType type)
    {
        if (StatFactors.ContainsKey(type)) return StatFactors[type];
        else return 1f;
    }

    public int GetRelativeValueOfType(AlignmentType typeToEeffect)
    {
         return SoulAlignment.GetRelativeValueOfType(typeToEeffect);
       
    }
    public int GetRangeValueOfType(AlignmentType typeToEeffect)
    {
        return SoulAlignment.GetValueOfAlignment(typeToEeffect);
    }

    public void EffectSoulALignment(int amount, AlignmentType typeToEeffect)
    {
        SoulAlignment.EffectRangeTowards(amount, typeToEeffect);
    }
    public void ApplyAlignmentChangeSuggestion(AlignmentChangeSuggestion alignmentChange)
    {
        float factor = GetStatFactorPercentageForAlignmentType(alignmentChange.AlignmentType);
        SoulAlignment.EffectRangeTowards((int)(alignmentChange.AmountToEffectBy * factor), alignmentChange.AlignmentType);
    }

   


    private bool AlignmentSuggestionIsWithinLimit(AlignmentType alignment, AlignmentConstraint constraint,int amount, int minLimitingAmount, int maxLimitingAmount)
    {
        //get the current alignment for this type
        int currenttALignmentValue = SoulAlignment.GetRelativeValueOfType(constraint.constrainingAlignmentType);

        //get the offset factor for this type
        float factor = GetStatFactorPercentageForAlignmentType(alignment);

        //multiply the amount and factor together 
        int AmountToEffectByApplied = Mathf.RoundToInt(amount * factor) * SoulAlignment.GetDirectionOfTypeForCalculations(constraint.constrainingAlignmentType);
        int soulValueToCompareTo = constraint.levelToCompare * SoulAlignment.GetDirectionOfTypeForCalculations(constraint.constrainingAlignmentType);

        // compare them together
        if (constraint.mathType == AlignmentConstraintType.max)
        {
            int AfterCalculationValue = currenttALignmentValue + AmountToEffectByApplied;
            if ((AfterCalculationValue > soulValueToCompareTo) || (AfterCalculationValue > maxLimitingAmount)) return false;
            else return true;
        }
        else 
        {
            int AfterCalculationValue = currenttALignmentValue - AmountToEffectByApplied;
            if ((soulValueToCompareTo < AfterCalculationValue) || (AfterCalculationValue < minLimitingAmount)) return false;
            else return true;
        }
       
    }

    public void HandleApplyingAlignmentChanges(List<AlignmentChangeSuggestion> changes)
    {
        Debug.Log(changes.Count);
        //organize the change suggestions into a dictionary and order each list from largest to smallest
        Dictionary<AlignmentType, List<AlignmentChangeSuggestion>> SuggestionDictionary = OrganizeAlignmentsByTypeAndAmount(changes);

        Dictionary < AlignmentType, int> minAmountPerType = new Dictionary<AlignmentType, int>();
        Dictionary < AlignmentType, int> maxAmountPerType = new Dictionary<AlignmentType, int>();

        //organize the min and max constraint amounts
        foreach (var suggestion in SuggestionDictionary)
        {
            Debug.Log(suggestion.Key);
            minAmountPerType.Add(suggestion.Key, 0);
            maxAmountPerType.Add(suggestion.Key, 100);

            foreach(var change in suggestion.Value)
            {
                if (change.ApplyTheseConstraintsToOtherSuggetions)
                {
                    foreach (var constraint in change.constraints)
                    {
                        if(constraint.mathType == AlignmentConstraintType.max)
                        {
                            maxAmountPerType[suggestion.Key] = Mathf.Max(maxAmountPerType[suggestion.Key], constraint.levelToCompare);
                        }
                        else
                        {
                            minAmountPerType[suggestion.Key] = Mathf.Min(maxAmountPerType[suggestion.Key], constraint.levelToCompare);
                        }
                    }
                }
            }
        }

        //iteratate over the entire dictionary and see what changes can be applied;
        foreach (var pair in SuggestionDictionary)
        {
            foreach (var change in pair.Value)
            {
                bool accepted = true;
                foreach (var constraint in change.constraints)
                {
                    int minamount = minAmountPerType[pair.Key];
                    int maxAmount = maxAmountPerType[pair.Key] != 0 ? maxAmountPerType[pair.Key] : 100;
                    if (!AlignmentSuggestionIsWithinLimit(pair.Key, constraint, change.AmountToEffectBy, minamount, maxAmount)) accepted = false;

                }
                if (accepted)
                {
                    Debug.Log("Accepted");
                    ApplyAlignmentChangeSuggestion(change);
                }
                else
                {
                    Debug.Log("rejected");
                }
            }
        }

    }


    private Dictionary<AlignmentType, List<AlignmentChangeSuggestion>> OrganizeAlignmentsByTypeAndAmount(List<AlignmentChangeSuggestion> listToOrganize)
    {
        Dictionary<AlignmentType, List<AlignmentChangeSuggestion>> dictionary = new();
        foreach (var item in listToOrganize)
        {
            AlignmentType type = item.AlignmentType;
            if (!dictionary.ContainsKey(type))
            {
                dictionary.Add(type, new List<AlignmentChangeSuggestion>());
                dictionary[type].Add(item);
            }
            else
            {
                dictionary[type].Add(item);
            }
        }

        foreach (var list in dictionary)
        {
            list.Value.OrderByDescending(i => i.AmountToEffectBy);
           
        }
        return dictionary;
    }

    public override string ToString()
    {
        string traits = "";
        foreach(var trait in SoulTraits)
        {
            traits += trait.Id + " ";
        }
        return $"{BackStory.ToString()}\n{SoulAlignment.ToString()}\n{traits}";
         
    }
}
