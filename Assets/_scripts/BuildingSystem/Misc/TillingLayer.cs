using GameSystems.BuildingSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace GameSystems.BuildingSystem
{
    public class TillingLayer : TileMapLayer
    {
        [SerializeField] private TileBase dryTile;
        [SerializeField] private TileBase wateredTile;
        private Dictionary<Vector3Int, TileBase> buildablesDictionary = new Dictionary<Vector3Int, TileBase>();
        private List<Vector3Int> wateredList = new List<Vector3Int>();
        public bool IsEmpty(Vector3 worldCoords)
        {
            var coords = TileMap.WorldToCell(worldCoords);

            return !buildablesDictionary.ContainsKey(coords) && TileMap.GetTile(coords) == null;

        }

        public bool IsWatered(Vector3 coords)
        {
            Vector3Int tilecoord = TileMap.WorldToCell(coords);
            return TileMap.GetTile(tilecoord) == wateredTile;
        }
        public void WaterSoil(Vector3 coords)
        {
            Debug.Log("watering");
            Vector3Int tilecoord = TileMap.WorldToCell(coords);
            if (TileMap.GetTile(tilecoord) == null) return;
            ConstructionLayerManager.Instance.RemoveTilledTileFromSaveData(tilecoord);
            wateredList.Add(tilecoord);
            SetTile(tilecoord, wateredTile);
            ConstructionLayerManager.Instance.AddTilledTileToSaveData(tilecoord, false);
        }
        public void TillSoil(Vector3 coords)
        {
            Vector3Int tilecoord = TileMap.WorldToCell(coords);
            SetTile(tilecoord, dryTile);
            ConstructionLayerManager.Instance.AddTilledTileToSaveData(tilecoord, true);
        }

        private void SetTile(Vector3Int coords, TileBase tile)
        {
            if(buildablesDictionary.ContainsKey(coords)) buildablesDictionary[coords] = tile;
            else buildablesDictionary.Add(coords, tile);
            TileMap.SetTile(coords, tile);
        }
        public void DestroyTile(Vector3 coords)
        {
            Vector3Int tilecoord = TileMap.WorldToCell(coords);
            buildablesDictionary.Remove(tilecoord);
            TileMap.SetTile(tilecoord, null);
           ConstructionLayerManager.Instance.RemoveTilledTileFromSaveData(tilecoord);
        }
    }
}