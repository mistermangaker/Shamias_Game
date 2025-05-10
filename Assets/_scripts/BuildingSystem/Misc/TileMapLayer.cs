using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameSystems.BuildingSystem
{
    [RequireComponent(typeof(Tilemap))]
    public class TileMapLayer : MonoBehaviour
    {
        protected Tilemap TileMap { get; private set; }

        protected virtual void Awake()
        {
            TileMap = GetComponent<Tilemap>();
        }
    }
}

