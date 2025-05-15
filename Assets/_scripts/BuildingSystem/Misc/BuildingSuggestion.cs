using UnityEngine;

namespace GameSystems.BuildingSystem
{
    public struct BuildingSuggestion
    {
        public Vector3 position { get; set; }
        public BuildableTiles BuildableTiles { get; set; }
        public BuildingType type { get; set; }

        public BuildingSuggestion(Vector3 position, BuildableTiles buildableTiles, BuildingType type = BuildingType.Spawned)
        {
            this.position = position;
            this.BuildableTiles = buildableTiles;
            this.type = type;
        }
    }
}


