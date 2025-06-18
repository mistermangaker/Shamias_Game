using UnityEngine;

namespace GameSystems.BuildingSystem
{
    public struct BuildingSuggestion
    {
        public bool RandomizeInitialConditions;
        public Vector3 position { get; set; }
        public BuildableTiles BuildableTiles { get; set; }
        public BuildingType type { get; set; }

        public BuildingSuggestion(Vector3 position, BuildableTiles buildableTiles, BuildingType type = BuildingType.Spawned, bool randomize= false)
        {
            this.RandomizeInitialConditions = randomize;
            this.position = position;
            this.BuildableTiles = buildableTiles;
            this.type = type;
        }
    }
}


