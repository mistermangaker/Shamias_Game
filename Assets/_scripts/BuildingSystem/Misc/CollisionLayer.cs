using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameSystems.BuildingSystem
{
    public class CollisionLayer : TileMapLayer
    {
        [SerializeField] private TileBase _collisionTile;

        public void SetCollisions(Buildable buildable, bool value)
        {
            var tile = value ? _collisionTile : null;
            buildable.IterateCollisionSpace(tileCoords => TileMap.SetTile(tileCoords, tile));
        }

    }
}

