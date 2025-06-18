using Callendar;
using GameSystems.Inventory;
using NUnit.Framework;
using System;
using UnityEngine;
using static UnityEditor.Progress;

public struct OnPlayerDeath : IEvent
{

}

//public class PlayerData
//{
//    public StatType playerMoveSpeed = new StatType(5f,5f);
//    public float playermovespeed => playerMoveSpeed.CurrentValue;

//    public StatType playerDamageAmount = new StatType(5f, 5f);
//    public int baseDamageAmount => (int)playerHealth.CurrentValue;

//    public StatType playerEnergy = new StatType(100, 100);
//    public int baseMaxEnergy => (int)playerEnergy.MaxValue;
//    public int currentEnergy => (int)playerEnergy.CurrentValue;

//    public StatType playerHealth = new StatType(100, 100);
//    public int baseMaxHealth => (int)playerHealth.MaxValue;
//    public int currentHealth => (int)playerHealth.CurrentValue;

//    public StatType playerHunger = new StatType(100, 100);
//    public int baseMaxHunger => (int)playerHunger.MaxValue;
//    public int currentHunger => (int)playerHunger.CurrentValue;

//    public Alignment currentPlayerAlignment =new Alignment();
//}
public class PlayerData
{

    public float playermovespeed;

    public int baseDamageAmount;


    public int baseMaxEnergy;
    public int currentEnergy;


    public int baseMaxHealth;
    public int currentHealth;

    public int baseMaxHunger;
    public int currentHunger;

    public int baseMaxSoulThirst;
    public int currentSoulThirst;
    public int defaultSoulThirst;

    public Alignment currentPlayerAlignment = new Alignment();
}
public class PlayerInternalState : MonoBehaviour, IDamagable
{
    public static PlayerInternalState Instance;


    public PlayerInventory inventory => PlayerInventory.Instance;
    private PlayerData _playerData = new PlayerData
    {
        playermovespeed = 5f,
        baseDamageAmount = 5,

        baseMaxEnergy = 100,
        currentEnergy = 100,

        baseMaxHealth = 100,
        currentHealth = 100,

        baseMaxHunger = 100,
        currentHunger = 100,

        baseMaxSoulThirst = 6,
        defaultSoulThirst = 3,
        currentSoulThirst = 3,
    };
    public PlayerData PlayerData => _playerData;

    private void Awake()
    {
        Instance = this;
    }


    public void ResetPlayerState()
    {
        _playerData.currentEnergy = _playerData.baseMaxEnergy;
        _playerData.currentHealth = _playerData.baseMaxHealth;
        _playerData.currentHunger = _playerData.baseMaxHunger;
        _playerData.currentSoulThirst = _playerData.defaultSoulThirst;
    }


    public int GetPlayerAlignmentFromType(AlignmentType type)
    {
        return _playerData?.currentPlayerAlignment.GetValueOfAlignment(type)?? 0;
    }

    public void Damage(int damage, DamageType damageType)
    {
        if(damageType == DamageType.Slash || damageType == DamageType.Blunt)
        {
            _playerData.currentHealth -= damage;
            if(_playerData.currentHealth <= 0)
            {
                EventBus<OnPlayerDeath>.Raise(new OnPlayerDeath());
            }
        }
    }
    public void ConsumeFood(InventorySlot slot)
    {
        GameItem item = slot.GameItem;
        if (item.GameItemData == null) return;
        if(!item.IsFoodItem && item.SoulData == null) return;
        if(item.IsFoodItem)
        {
            EffectPlayerEnergy(item.GameItemData.Cosnumeables.EnergyOffset);
            EffectPlayerHealth(item.GameItemData.Cosnumeables.HealthOffset);
            EffectPlayerHunger(item.GameItemData.Cosnumeables.HungerOffset);
        }
        if(item.SoulData != null)
        {
            ConsumeSoul(item.SoulData);
        }
        slot.RemoveFromStack(1);
    }
    public void EffectPlayerEnergy(int amount)
    {
        _playerData.currentEnergy = Mathf.Clamp(_playerData.currentEnergy + amount, 0, _playerData.baseMaxEnergy);
    }
    public void EffectPlayerHunger(int amount)
    {
        _playerData.currentHunger = Mathf.Clamp(_playerData.currentHunger + amount, 0, _playerData.baseMaxHunger);
    }
    public void EffectPlayerHealth(int amount)
    {
        _playerData.currentHealth = Mathf.Clamp(_playerData.currentHealth + amount, 0, _playerData.baseMaxHealth);
    }
    public void EffectPlayerSoulThirst(int amount)
    {
        _playerData.currentSoulThirst = Mathf.Clamp(_playerData.currentSoulThirst + amount, 0, _playerData.baseMaxSoulThirst);
    }
    public void ConsumeSoul(SoulData soulData)
    {
        Alignment alignment  =soulData.SoulAlignment;
        float percentage = 0.1f;
        float Chastity  = (float)alignment.GetValueOfAlignment(AlignmentType.Chastity) * percentage;
        float Temperance = (float)alignment.GetValueOfAlignment(AlignmentType.Temperance) * percentage;
        float Forgivness = (float)alignment.GetValueOfAlignment(AlignmentType.Forgivness) * percentage;
        float Charity = (float)alignment.GetValueOfAlignment(AlignmentType.Charity) * percentage;
        float Diligence = (float)alignment.GetValueOfAlignment(AlignmentType.Diligence) * percentage;
        float Humility = (float)alignment.GetValueOfAlignment(AlignmentType.Humility) * percentage;
        float Kindness = (float)alignment.GetValueOfAlignment(AlignmentType.Kindness) * percentage;

        _playerData.currentPlayerAlignment.EffectRangeTowards((int)Chastity, AlignmentType.Chastity);
        _playerData.currentPlayerAlignment.EffectRangeTowards((int)Temperance, AlignmentType.Temperance);
        _playerData.currentPlayerAlignment.EffectRangeTowards((int)Forgivness, AlignmentType.Forgivness);
        _playerData.currentPlayerAlignment.EffectRangeTowards((int)Charity, AlignmentType.Charity);
        _playerData.currentPlayerAlignment.EffectRangeTowards((int)Diligence, AlignmentType.Diligence);
        _playerData.currentPlayerAlignment.EffectRangeTowards((int)Humility, AlignmentType.Humility);
        _playerData.currentPlayerAlignment.EffectRangeTowards((int)Kindness, AlignmentType.Kindness);
        EffectPlayerSoulThirst(1);
    }

    public float PlayerMoveSpeed => _playerData != null ? _playerData.playermovespeed : 5f;
   
    public int PlayerDamageAmount => _playerData != null ? _playerData.baseDamageAmount : 0;
    
    public int PlayerMaxEnergy => _playerData != null ? _playerData.baseMaxEnergy : 0;
   
    public int PlayerMaxHealth =>  _playerData != null ? _playerData.baseMaxHealth : 100;

    public int PlayerMaxSoulThirst => _playerData != null ? _playerData.baseMaxSoulThirst : 6;

    public int PlayerCurrentEnergy => _playerData != null ? _playerData.currentEnergy : 0;

    public int PlayerCurrentHealth => _playerData !=null ? _playerData.currentHealth : 0;

    public int PlayerCurrentSoulThirst => _playerData !=null ? _playerData.currentSoulThirst : 0;
   
    public Alignment PlayerCurrentAlignment => _playerData!=null? _playerData.currentPlayerAlignment : new Alignment();
    

}
