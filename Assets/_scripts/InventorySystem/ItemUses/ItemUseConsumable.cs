using GameSystems.Inventory;
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemUse", menuName = "Inventory System/Items/ItemUseConsumable")]
public class ItemUseConsumable : ItemUse
{
    [field: SerializeField] public int HungerOffset { get; private set; }
    [field: SerializeField] public int EnergyOffset { get; private set; }
    [field: SerializeField] public int HealthOffset { get; private set; }


    public override string GetEffectsName(GameItem item)
    {
        string consumable = $"Consumeable: \n" +
            $"Hunger: {HungerOffset}\n" +
            $"Energy: {EnergyOffset}\n" +
            $"Health: {HealthOffset}\n";
        if (item.SoulData != null)
        {
            consumable += $"{item.SoulData.SoulAlignment.ToString()}";
        }
        return consumable;
    }

    public override void Use(GameItem item)
    {
        PlayerInternalState.Instance.EffectPlayerEnergy(EnergyOffset);
        PlayerInternalState.Instance.EffectPlayerHealth(HealthOffset);
        PlayerInternalState.Instance.EffectPlayerHunger(HungerOffset);
        if(item.SoulData != null)
        {
            PlayerInternalState.Instance.ConsumeSoul(item.SoulData);
        }
    }
}
