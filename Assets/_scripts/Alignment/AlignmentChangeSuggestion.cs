using System;
using System.Collections.Generic;

[Serializable]
public class AlignmentChangeSuggestion
{
    [NonSerialized]public int SortOrder;
    public int AmountToEffectBy;
    public AlignmentType AlignmentType;
    public List<AlignmentConstraint> constraints;
    public bool ApplyTheseConstraintsToOtherSuggetions;
}
