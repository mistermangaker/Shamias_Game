using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameSystems.BuildingSystem
{

    public class MaskLayer : TileMapLayer
    {
        private List<Vector3Int> _maskLayerTiles;
        public List<Vector3Int> MaskLayerTiles
        {
            get
            {
                if (_maskLayerTiles == null)
                {
                    _maskLayerTiles = new List<Vector3Int>();
                    _maskLayerTiles = GetAllTilesInLayer();
                }
                return _maskLayerTiles;
            }
        }
        public List<Vector3Int> GetAllTilesInLayer()
        {
            List<Vector3Int> positions = new List<Vector3Int>();
            BoundsInt bounds = TileMap.cellBounds;
            for (int x = bounds.min.x; x <= bounds.max.x; x++)
            {
                for (int y = bounds.min.y; y <= bounds.max.y; y++)
                {
                    TileBase temp = TileMap.GetTile(new Vector3Int(x, y, 0));
                    if (temp != null)
                    {
                        positions.Add(new Vector3Int(x, y, 0));
                    }
                }
            }
            return positions;
        }


        public Vector3Int GetRandomTileCoords()
        {
            int rand = Random.Range(0, _maskLayerTiles.Count);
            return _maskLayerTiles[rand];
        }

        public bool HasTileAtPosition(Vector3 position)
        {
            var coords = TileMap.WorldToCell(position);

            return TileMap.GetTile(coords) != null;
        }

       
    }
}