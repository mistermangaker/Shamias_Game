using System;

[Serializable]
public struct AlignmentConstraint
{
    public int levelToCompare;
    public AlignmentType constrainingAlignmentType;
    public AlignmentConstraintType mathType;
}


public enum AlignmentConstraintType
{
    max,
    min,
}
