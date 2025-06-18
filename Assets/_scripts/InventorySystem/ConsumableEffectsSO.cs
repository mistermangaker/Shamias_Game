using UnityEngine;

//[CreateAssetMenu(fileName = "ConsumableEffectsSO", menuName = "Scriptable Objects/ConsumableEffectsSO")]
public class ConsumableEffectsSO : ScriptableObjectWithId
{
    [field:SerializeField] public int HungerOffset {  get; private set; }
    [field:SerializeField] public int EnergyOffset {  get; private set; }
    [field:SerializeField] public int HealthOffset {  get; private set; }
}
