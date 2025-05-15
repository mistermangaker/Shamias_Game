using GameSystems.Inventory;
using System;
using UnityEditor.Playables;
using UnityEngine;

[Serializable]
public class Alignment
{
    
    private Alignment_Range chastity_Lust { get; set; }
    public Alignment_Range Chastity_Lust => chastity_Lust;
   
    private Alignment_Range forgiveness_Wrath { get; set; }
    public Alignment_Range Forgivness_Wrath => forgiveness_Wrath;
    
    private Alignment_Range temperance_Gluttony { get; set; }
    public Alignment_Range Temperance_Gluttony => temperance_Gluttony;
   
    private Alignment_Range charity_Greed { get; set; }
    public Alignment_Range Charity_Greed => charity_Greed;
    
    private Alignment_Range diligence_Sloth { get; set; }
    public Alignment_Range Diligence_Sloth => diligence_Sloth;
    private Alignment_Range kindness_Envy { get; set; }
    public Alignment_Range Kindness_Envy => kindness_Envy;
    private Alignment_Range humility_Pride { get; set; }
    public Alignment_Range Humility_Pride => humility_Pride;

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
public struct Alignment_Range
{
    private int _rangeValue;
  
    public int RangeValue
    {
        get
        {
            return Mathf.Clamp(_rangeValue, _rangeMin, _rangeMax);
        }
        set
        {
            _rangeValue = Mathf.Clamp(value, _rangeMin, _rangeMax);
        }
    }

    private int _rangeMin;
    private int _rangeMax;

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
