using GameSystems.Inventory;
using UnityEngine;


public class PlayerData
{
    public int playermovespeed;
}

public class PlayerInternalState : MonoBehaviour
{
    public static PlayerInternalState Instance;
    private PlayerInventory inventory => PlayerInventory.Instance;
    private PlayerData _playerData;
    public PlayerData PlayerData => _playerData;

    private void Awake()
    {
        Instance = this;
        _playerData = new PlayerData();
    }
    
}
