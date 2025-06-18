using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoulBackStoryPart", menuName = "Scriptable Objects/SoulBackStoryPart")]
public class SoulBackStoryPart : ScriptableObject
{
    [field: SerializeField] public string Id { get; private set; }
    [field: SerializeField, TextArea(4, 4)] public string BackGroundString { get; private set; }
    [field: SerializeField] public SoulBackStoryLifeTimeTag LifeStageTag { get; private set; }
    [field: SerializeField] public List<AlignmentAdjustment> ConnectedAlignments { get; private set; }
    [field: SerializeField] public List<ThingTag> ThingTags { get; private set; }
    [field: SerializeField] public List<ThingAmount<SoulTraitsForReference>> ForcedTraits { get; private set; }

    public SoulTrait GetSoulTraitAtIndex(int index)
    {
        index = Mathf.Clamp(index, 0, ForcedTraits.Count);
        return ForcedTraits[index].Thing.GetSoulTraitAtIndex(ForcedTraits[index].amount);
    }

    public List<SoulTrait> GetAllSoulTraits()
    {
        List<SoulTrait> list = new List<SoulTrait>();
        for (int i = 0; i < ForcedTraits.Count; i++)
        {
            list.Add(GetSoulTraitAtIndex(i));
        }
        return list;
    }


}

[Serializable]
public class ThingAmount<T>
{
    public T Thing;
    public int amount;

}
[Serializable]
public struct ValuePair<T, M>
{
    public T Value;
    public M Amount;
}
public enum SoulBackStoryLifeTimeTag
{
    ChildHood,
    AdultHood,
    DeathCause
}

[Serializable]
public struct AlignmentAdjustment
{
    public SoulMathType applicationType;
    public AlignmentType TypeToModify;
    public int amountToModify;
    
}

public enum SoulMathType
{
    StatOffsetPercentage,
    StatFactorPercentage
}

[Serializable]
public class StatStages<T>
{
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField, TextArea(4, 4)] public string Description { get; private set; }
    public T Value;
}

