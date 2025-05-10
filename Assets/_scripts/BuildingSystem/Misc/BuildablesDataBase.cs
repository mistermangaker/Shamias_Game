using GameSystems.Inventory;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace GameSystems.BuildingSystem
{
    [CreateAssetMenu(menuName = "Building System/Buildables Database")]
    public class BuildablesDataBase : ScriptableObject
    {
        [SerializeField] private List<BuildableTiles> _buildablesDataBase;

        [ContextMenu("Set Ids")]
        public void SetItemIDs()
        {
            _buildablesDataBase = new List<BuildableTiles>();

            var founditems = Resources.LoadAll<BuildableTiles>("BuildableTilesDataBase").OrderBy(i => i.TileName).ToList();
            _buildablesDataBase.AddRange(founditems);
        }

        public BuildableTiles GetBuildable(string itemID)
        {
            return _buildablesDataBase.Find(i => i.TileName == itemID);
        }

    }
}