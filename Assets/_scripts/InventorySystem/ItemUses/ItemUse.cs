using GameSystems.Inventory;
using UnityEngine;

//[CreateAssetMenu(fileName = "ItemUse", menuName = "Inventory System/Items/ItemUse")]
public abstract class ItemUse : ScriptableObject
{
    public abstract string GetEffectsName(GameItem item);
    public abstract void Use(GameItem item);
}
