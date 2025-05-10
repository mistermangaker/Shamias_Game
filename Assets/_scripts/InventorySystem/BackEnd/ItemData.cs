
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.Inventory
{
    [CreateAssetMenu(menuName ="Inventory System/New ItemData")]
    public class ItemData : ScriptableObject
    {
        [field:SerializeField] public string ItemId { get; private set; }
        [field:SerializeField] public string DisplayName { get; private set; }
        [field:SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public int MaxStackSize { get; private set; }

        [TextArea(4, 4)] public string ItemDescription;


        [field: SerializeField] public int DefaultDurability { get; private set; }
        [field: SerializeField] public bool UsesTimeStamps { get; private set; }

        [field: SerializeField] public BuildableTiles Buildable {  get; private set; }

        [field: SerializeField] public InteractionIntent InteractionIntent { get; private set; } = InteractionIntent.None;
        [field: SerializeField] public List<InteractionIntent> InteractionIntents { get; private set; } 

        [field: SerializeField] public bool ConsumeableOnUse { get; private set; }

        [field: SerializeField] public int DefaultSellPrice { get; private set; }

    }
}



