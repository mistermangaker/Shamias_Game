using UnityEngine;
using UnityEngine.Events;

public class BuildingHealth : MonoBehaviour
{
    public UnityAction OnBuildingDeath;

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
    public void TryDamage(int damage)
    {
        if (_maxHealth <= 0)
        {
            Debug.LogWarning($"building: {gameObject.name} was damaged without first being initialized");
            InitializeHealth(100);
        }
        CurrentHealth -= damage;
    }
    public void InitializeHealth(int health)
    {
        _maxHealth = health;
        _currentHealth = health;
    }
}
