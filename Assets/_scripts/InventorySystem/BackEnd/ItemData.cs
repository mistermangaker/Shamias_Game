
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

        
        [field: SerializeField] public List<InteractionIntent> SecondaryInteractionIntents { get; private set; } 
        [field: SerializeField] public List<InteractionIntent> PrimaryInteractionIntents { get; private set; } 

        [field: SerializeField] public bool ConsumeableOnUse { get; private set; }

        [field: SerializeField] public int DefaultSellPrice { get; private set; }
        [field: SerializeField] public Alignment ItemAlignment { get; private set; }
        [field: SerializeField] public int ItemAttackDamage { get; private set; }

        
    }
}



