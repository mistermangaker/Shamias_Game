using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;


public abstract class BaseSoulBaseStoryPreset : ScriptableObject
{
    public abstract SoulBackStory GenerateBackStoryFromPreset();

    public abstract List<SoulTrait> GenerateSoulTraitFromPreset(int number);
}
