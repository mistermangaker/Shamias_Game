using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameSystems.Inventory
{
    [CreateAssetMenu(menuName = "Inventory System/Item Database")]
    public class ItemDataBase : ScriptableObject
    {
        [SerializeField] private List<ItemData> _itemDatabase;

        [ContextMenu("Set Ids")]
        public void SetItemIDs()
        {
            _itemDatabase = new List<ItemData>();

            var founditems = Resources.LoadAll<ItemData>("ItemData").OrderBy(i => i.ItemId).ToList();
            _itemDatabase.AddRange(founditems);
        }

        public ItemData GetItem(string itemID)
        {
            if(itemID == "null") return null;
            if (string.IsNullOrEmpty(itemID)) return null;
            return _itemDatabase.Find(i => i.ItemId == itemID);
        }

    }
}
