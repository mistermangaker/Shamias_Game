using GameSystems.Inventory;
using System;
using UnityEditor.Playables;
using UnityEngine;


public enum AlignmentType
{
    Chastity,Lust, Forgivness,Wrath, Temperance,Gluttony, Charity,Greed, Diligence,Sloth, Kindness,Envy, Humility,Pride
}
[Serializable]
public class Alignment
{
    
   [field: SerializeField] private Alignment_Range chastity_Lust { get; set; }
    public Alignment_Range Chastity_Lust => chastity_Lust;

    [field: SerializeField] private Alignment_Range forgiveness_Wrath { get; set; }
    public Alignment_Range Forgivness_Wrath => forgiveness_Wrath;

    [field: SerializeField] private Alignment_Range temperance_Gluttony { get; set; }
    public Alignment_Range Temperance_Gluttony => temperance_Gluttony;

    [field: SerializeField] private Alignment_Range charity_Greed { get; set; }
    public Alignment_Range Charity_Greed => charity_Greed;

    [field: SerializeField] private Alignment_Range diligence_Sloth { get; set; }
    public Alignment_Range Diligence_Sloth => diligence_Sloth;
    [field: SerializeField] private Alignment_Range kindness_Envy { get; set; }
    public Alignment_Range Kindness_Envy => kindness_Envy;
    [field: SerializeField] private Alignment_Range humility_Pride { get; set; }
    public Alignment_Range Humility_Pride => humility_Pride;

    public void EffectRangeTowards(int amount, AlignmentType type)
    {
        switch (type)
        {
            default:
            case AlignmentType.Lust:
                chastity_Lust.ChangeAlignment(-amount);
                break;
            case AlignmentType.Chastity:
                chastity_Lust.ChangeAlignment(amount);
                break;
            case AlignmentType.Forgivness:
                forgiveness_Wrath.ChangeAlignment(amount);
                break;
            case AlignmentType.Wrath:
                forgiveness_Wrath.ChangeAlignment(-amount);
                break;
            case AlignmentType.Temperance:
                temperance_Gluttony.ChangeAlignment(amount);
                break;
            case AlignmentType.Gluttony:
                temperance_Gluttony.ChangeAlignment(-amount);
                break;
            case AlignmentType.Charity:
                charity_Greed.ChangeAlignment(amount);
                break;
            case AlignmentType.Greed:
                charity_Greed.ChangeAlignment(-amount);
                break;
            case AlignmentType.Diligence:
                diligence_Sloth.ChangeAlignment(amount);
                break;
            case AlignmentType.Sloth:
                diligence_Sloth.ChangeAlignment(-amount);
                break;
            case AlignmentType.Kindness:
                kindness_Envy.ChangeAlignment(amount);
                break;
            case AlignmentType.Envy:
                kindness_Envy.ChangeAlignment(-amount);
                break;
            case AlignmentType.Humility:
                humility_Pride.ChangeAlignment(amount);
                break;
            case AlignmentType.Pride:
                humility_Pride.ChangeAlignment(-amount);
                break;
        }
    }
    public int GetDirectionOfTypeForCalculations(AlignmentType type)
    {
        switch (type)
        {
            default:
            case AlignmentType.Chastity:
                return 1;
            case AlignmentType.Lust:
                return -1;
            case AlignmentType.Forgivness:
                return 1;
            case AlignmentType.Wrath:
                return -1;
            case AlignmentType.Temperance:
                return 1;
            case AlignmentType.Gluttony:
                return -1;
            case AlignmentType.Charity:
                return 1;
            case AlignmentType.Greed:
                return -1;
            case AlignmentType.Diligence:
                return 1;
            case AlignmentType.Sloth:
                return -1;
            case AlignmentType.Kindness:
                return 1;
            case AlignmentType.Envy:
                return -1;
            case AlignmentType.Humility:
                return 1;
            case AlignmentType.Pride:
                return -1;
        }
    }

    public int GetRelativeValueOfType(AlignmentType type)
    {
        switch (type)
        {
            default:
            case AlignmentType.Chastity:
                return chastity_Lust.RangeValue;
            case AlignmentType.Lust:
                return -chastity_Lust.RangeValue;
            case AlignmentType.Forgivness:
                return forgiveness_Wrath.RangeValue; 
            case AlignmentType.Wrath:
                return -forgiveness_Wrath.RangeValue;
            case AlignmentType.Temperance:
                return temperance_Gluttony.RangeValue;
            case AlignmentType.Gluttony:
                return -temperance_Gluttony.RangeValue;
            case AlignmentType.Charity:
                return charity_Greed.RangeValue;
            case AlignmentType.Greed:
                return -charity_Greed.RangeValue;
            case AlignmentType.Diligence:
                return Diligence_Sloth.RangeValue;
            case AlignmentType.Sloth:
                return -Diligence_Sloth.RangeValue;
            case AlignmentType.Kindness:
                return kindness_Envy.RangeValue;
            case AlignmentType.Envy:
                return -kindness_Envy.RangeValue;
            case AlignmentType.Humility:
                return humility_Pride.RangeValue;
            case AlignmentType.Pride:
                return -humility_Pride.RangeValue;
        }
    }
    public int GetValueOfAlignment(AlignmentType type)
    {
        return GetAlignmentRange(type).RangeValue;
    }
    public Alignment_Range GetAlignmentRange(AlignmentType type)
    {
        switch (type)
        {
            default:
            case AlignmentType.Lust:
                return chastity_Lust;
            case AlignmentType.Chastity:
                return chastity_Lust;
            case AlignmentType.Forgivness:
                return forgiveness_Wrath;
            case AlignmentType.Wrath:
                return forgiveness_Wrath;
            case AlignmentType.Temperance:
                return temperance_Gluttony;
            case AlignmentType.Gluttony:
                return temperance_Gluttony;
            case AlignmentType.Charity:
                return charity_Greed;
            case AlignmentType.Greed:
                return charity_Greed;
            case AlignmentType.Diligence:
                return diligence_Sloth;
            case AlignmentType.Sloth:
                return diligence_Sloth;
            case AlignmentType.Kindness:
                return kindness_Envy;
            case AlignmentType.Envy:
                return kindness_Envy;
            case AlignmentType.Humility:
                return humility_Pride;
            case AlignmentType.Pride:
                return humility_Pride;
        }
    }

    public Alignment(int range, int chasity, int forgiveness, int temperance,int charity, int dilligence , int kindness , int humility)
    {
        chastity_Lust = new Alignment_Range(range, chasity);
        forgiveness_Wrath = new Alignment_Range(range, forgiveness);
        temperance_Gluttony = new Alignment_Range(range, temperance);
        charity_Greed = new Alignment_Range(range, charity);
        diligence_Sloth = new Alignment_Range(range, dilligence);
        kindness_Envy = new Alignment_Range(range, kindness);
        humility_Pride = new Alignment_Range(range, humility);
    }
    public Alignment()
    {
        chastity_Lust = new Alignment_Range(100, 0);
        forgiveness_Wrath = new Alignment_Range(100, 0);
        temperance_Gluttony = new Alignment_Range(100, 0);
        charity_Greed = new Alignment_Range(100, 0);
        diligence_Sloth = new Alignment_Range(100, 0);
        kindness_Envy = new Alignment_Range(100, 0);
        humility_Pride = new Alignment_Range(100, 0);
    }

    public override bool Equals(object obj)
    {
        return obj is Alignment alignment && Equals(alignment);
    }

    public static bool operator ==(Alignment left, Alignment right) => left.Equals(right);
    public static bool operator !=(Alignment left, Alignment right) => !(left == right);


    public bool Equals(Alignment other)
    {
        return other.Chastity_Lust == this.Chastity_Lust &&
             other.Forgivness_Wrath == this.Forgivness_Wrath &&
             other.Temperance_Gluttony == this.Temperance_Gluttony &&
             other.Charity_Greed == this.charity_Greed &&
             other.Diligence_Sloth == this.diligence_Sloth &&
             other.kindness_Envy == this.kindness_Envy &&
             other.Humility_Pride == this.humility_Pride;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Chastity_Lust, Forgivness_Wrath, Temperance_Gluttony, Charity_Greed, Diligence_Sloth, Kindness_Envy, Humility_Pride);
    }

    public override string ToString()
    {
        return $"Chastity/Lust: {Chastity_Lust} Forgiveness/Wrath: {Forgivness_Wrath} Temperance/Gluttony: {Temperance_Gluttony} " +
            $"\n Charity/Greed: {Charity_Greed} Diligence/Sloth: {Diligence_Sloth} Kindness/Envy: {Kindness_Envy}" +
            $"\n Humility/Pride: {Humility_Pride}";
    }
}

[Serializable]
public class Alignment_Range
{
    [SerializeField] private int _rangeValue;

    public int RangeValue => _rangeValue;
 
    [SerializeField] private int _rangeMin;
    [SerializeField] private int _rangeMax;

    public Alignment_Range(int range,int rangeValue =0)
    {
        _rangeMin = -range;
        _rangeMax = range;
        _rangeValue = rangeValue;
    }

    public Alignment_Range(int min, int max, int rangeValue = 0)
    {
        _rangeMin = min;
        _rangeMax = max;
        _rangeValue = rangeValue;
    }

    public void ChangeAlignment(int amount)
    {
        _rangeValue += amount;
        _rangeValue = Mathf.Clamp(_rangeValue, _rangeMin, _rangeMax);
    }

    public static bool operator ==(Alignment_Range left, Alignment_Range right) => left.Equals(right);
    public static bool operator !=(Alignment_Range left, Alignment_Range right) => !(left == right);

    public override bool Equals(object obj)
    {
        return obj is Alignment_Range range && range.RangeValue == this.RangeValue;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_rangeValue, _rangeMin, _rangeMax);
    }
    public override string ToString()
    {
        return RangeValue.ToString();
    }
}
