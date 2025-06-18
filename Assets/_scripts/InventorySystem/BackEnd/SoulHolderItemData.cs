using UnityEngine;

namespace GameSystems.Inventory
{
    
    [CreateAssetMenu(menuName = "Inventory System/New SoulHolderItemData")]
    public class SoulHolderItemData : ItemData
    {
        [field:SerializeField] public SoulData soulData { get; private set; }
    }
}


