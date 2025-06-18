using Callendar;
using System.Collections.Generic;
using UnityEngine;

public enum StatModierType
{
    Offset,
    Factor,
}

public struct StatModifier
{
    public string ID;
    public StatModierType Type;
    public float Value;
    public DateTimeStamp ShouldRemoveTime;

}
public struct StatType
{
    [field: SerializeField] public float BaseMaxValue { get; private set; }
    [field: SerializeField] public float BaseCurrentValue { get; private set; }
   
    public float CurrentValue => GetRealStatValuesForCurrentValue();
    public float MaxValue => GetRealStatValuesForMaxValue();

    private List<StatModifier> MaxValueModifiers; 
    private List<StatModifier> CurrentValueModifiers; 


    public StatType(float baseMaxValue, float currentValue)
    {
        BaseMaxValue = baseMaxValue;
        BaseCurrentValue = currentValue;
        MaxValueModifiers = new List<StatModifier>();
        CurrentValueModifiers = new List<StatModifier>();
    }
     
    private void Authenticate()
    {
        List<StatModifier> list = new List<StatModifier>();
        foreach (var modifiers in MaxValueModifiers)
        {
            if (modifiers.ShouldRemoveTime <= TimeManager.Instance.CurrentGameTime)
            {
                list.Add(modifiers);
                
            }
        }
        foreach (var modifier in list)
        {
            MaxValueModifiers.Remove(modifier);
        }
        list.Clear();
        foreach (var modifiers in CurrentValueModifiers)
        {
            if (modifiers.ShouldRemoveTime <= TimeManager.Instance.CurrentGameTime)
            {
                list.Add(modifiers);

            }
        }
        foreach (var modifier in list)
        {
            CurrentValueModifiers.Remove(modifier);
        }
    }


    public float GetRealStatValuesForMaxValue()
    {
        return GetRealStatValuesForValue(BaseMaxValue, MaxValueModifiers);
    }
    public float GetRealStatValuesForCurrentValue()
    {
        return GetRealStatValuesForValue(BaseCurrentValue, CurrentValueModifiers);
    }

    private float GetRealStatValuesForValue(float value, List<StatModifier> modifiers)
    {
        Authenticate();
        List<StatModifier> offsets = new List<StatModifier>();
        List<StatModifier> factors = new List<StatModifier>();
        foreach (var modifier in modifiers)
        {
            if (modifier.Type == StatModierType.Offset) offsets.Add(modifier);
            else factors.Add(modifier);
        }
        float totalFactors = 0f;
        foreach (var modifier in factors)
        {
            totalFactors += modifier.Value;
        }
        float totaloffsets = 0f;
        foreach (var modifier in offsets)
        {
            totaloffsets += modifier.Value;
        }

        return (value + totaloffsets) * totalFactors;
    }
    public void AddCurrentStatModifier(StatModifier modifier)
    {
        AddStatModifier(modifier, CurrentValueModifiers);
    }
    public void RemoveCurrentStatModifier(StatModifier modifier)
    {
        RemoveStatModifier(modifier, CurrentValueModifiers);
    }
    public void AddMaxStatModifier(StatModifier modifier)
    {
        AddStatModifier(modifier, MaxValueModifiers);
    }
    public void RemoveMaxStatModifier(StatModifier modifier)
    {
        RemoveStatModifier(modifier, MaxValueModifiers);
    }

    private void RemoveStatModifier(StatModifier modifier, List<StatModifier> list)
    {
        foreach (var modifiers in list)
        {
            if (modifier.ID == modifiers.ID)
            {
                list.Remove(modifiers);
                return;
            }
        }
    }
    private void AddStatModifier(StatModifier modifier, List<StatModifier>list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (modifier.ID == list[i].ID)
            {
                list[i] = modifier;
                return;
            }
        }
        list.Add(modifier);
    }
}
