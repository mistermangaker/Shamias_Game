using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "Enviroment/Biome")]
public class Biome : ScriptableObjectWithId
{
    [field:SerializeField] public List<ThingTag> ThingTags {  get; private set; }
    [field:SerializeField] public List<ThingAmount<BuildableTiles>> AllSeasonForageables {  get; private set; }
    [field:SerializeField] public List<ThingAmount<BuildableTiles>> SpringForageables {  get; private set; }
    [field:SerializeField] public List<ThingAmount<BuildableTiles>> SummerForageables {  get; private set; }
    [field:SerializeField] public List<ThingAmount<BuildableTiles>> FallForageables {  get; private set; }
    [field:SerializeField] public List<ThingAmount<BuildableTiles>> WinterForageables {  get; private set; }
    [field:SerializeField] public List<ThingAmount<BaseSoulBaseStoryPreset>> TypesOfSoulsToSpawn {  get; private set; }

}
