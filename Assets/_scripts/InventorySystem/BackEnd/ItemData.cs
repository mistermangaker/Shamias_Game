
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.Inventory
{
    [CreateAssetMenu(menuName ="Inventory System/New GameItemData")]
    public class ItemData : ScriptableObject
    {
        [field:SerializeField] public string ItemId { get; private set; }
        [field:SerializeField] public string DisplayName { get; private set; }
        [field:SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public int MaxStackSize { get; private set; } = 1;

        [TextArea(4, 4)] public string ItemDescription;


        [field: SerializeField] public int DefaultDurability { get; private set; }
        [field: SerializeField] public bool UsesTimeStamps { get; private set; }

        [field: SerializeField] public BuildableTiles Buildable {  get; private set; }

       

        [field: SerializeField] public InteractionIntent PrimaryInteractionIntent { get; private set; } 

        [field: SerializeField] public bool ConsumeableOnUse { get; private set; }

         public ConsumableEffectsSO Cosnumeables { get; private set; }

        [field: SerializeField] public int DefaultSellPrice { get; private set; }
       // [field: SerializeField] public Alignment ItemAlignment { get; private set; }
        [field: SerializeField] public int ItemAttackDamage { get; private set; }
        [field: SerializeField] public DamageType DamageType { get; private set; }
        [field: SerializeField] public float WeaponReach { get; private set; } = 1f;

        [field: SerializeField] public List<ItemUse> ItemUses { get; private set; }
    }
}



