using GameSystems.Inventory;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemFilter", menuName = "Scriptable Objects/ItemFilter")]
public class ItemFilter : ScriptableObject
{
    [field: SerializeField] public bool IsNegtiveFilter {  get; private set; }
    [field: SerializeField] public List<ItemData> ItemFilterList { get; private set; }

    public bool Contains(ItemData item)
    {
        foreach (ItemData item2 in ItemFilterList)
        {
            if(item ==  item2) return true;
        }
        return false;
    }
}
