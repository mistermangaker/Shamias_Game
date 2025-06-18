using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Events;

public class DamageableHealth : MonoBehaviour, IDamagable
{
    public UnityAction OnBuildingDeath;
   [SerializeField] private List<ValuePair<DamageType, float>> damageFactors = new List<ValuePair<DamageType, float>>();
    private int _maxHealth;
    public int MaxHealth {  get { return _maxHealth; } }
    private int _currentHealth;
    public int CurrentHealth {  
        get { return _currentHealth; } 
        private set { 
            _currentHealth = value; 
            if(_currentHealth < 0) 
                OnBuildingDeath?.Invoke(); 
        } }

    private int CalculateDamageAmount(int damage, DamageType damageType)
    {
       
        if(damageFactors.Count == 0)
        {
            return damage;
        } 
        float damageFactor = 1;
        bool flag = false;
        foreach (var pair in damageFactors)
        {
            if(damageType == pair.Value)
            {
                flag = true;
                damageFactor += pair.Amount;
                break;
            }
        }
        if(!flag)
        {
            return 0;
        }
        return Mathf.RoundToInt(damage * damageFactor);
    }

    public void InitializeHealth(int health, List<ValuePair<DamageType,float>> damageFactors = null)
    {
        if(damageFactors != null) this.damageFactors = damageFactors;
        _maxHealth = health;
        _currentHealth = health;
    }
  

    public void Damage(int damage, DamageType damageType = DamageType.Blunt)
    {
       
        if (_maxHealth <= 0)
        {
            Debug.LogWarning($"building: {gameObject.name} was damaged without first being initialized");
            InitializeHealth(100);
        }
        int damageAmountToAdd = CalculateDamageAmount(damage, damageType);
        Debug.Log("damaging " + damageAmountToAdd);
        CurrentHealth -= damageAmountToAdd;
    }
}
