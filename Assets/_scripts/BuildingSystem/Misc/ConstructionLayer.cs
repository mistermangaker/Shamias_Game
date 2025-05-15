using GameSystems.SaveLoad;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameSystems.BuildingSystem
{
    public class ConstructionLayer : TileMapLayer
    {
        private Dictionary<Vector3Int, Buildable> buildablesDictionary = new Dictionary<Vector3Int, Buildable>();
        public void BuildTile(BuildingSuggestion suggestion)
        {
            BuildTile(suggestion, out Buildable newBuildable);
        }
        public void BuildTile(BuildingSuggestion suggestion, out Buildable newBuildable)
        {
            newBuildable = null;    
            Vector3 worldCoords = suggestion.position;
            BuildableTiles item = suggestion.BuildableTiles;
            GameObject itemObject = null;
            var coords = TileMap.WorldToCell(worldCoords);

            var buildingType = suggestion.type;

            if (item.TileToPlace != null)
            {
                var tileChangeData = new TileChangeData(
                    coords,
                    item.TileToPlace,
                    Color.white,
                    Matrix4x4.Translate(item.TileOffset)
                    );

                TileMap.SetTile(tileChangeData, false);
            }
            if (item.BuildableGameObject != null)
            {
                float offset = TileMap.cellSize.x / 2;
                Vector3 worldoffset = new Vector3(offset, offset, offset);
                itemObject = Instantiate(
                    item.BuildableGameObject,
                    TileMap.CellToWorld(coords) + worldoffset + (Vector3)item.TileOffset, Quaternion.identity
                    );
            }
            var buildable = new Buildable(item, coords, TileMap, itemObject, buildingType);
            if (buildable.BuildableGameObject != null)
            {
                buildable.InitializeNewBuildableGameObjectSerializableGuid();
                BuildingBase building = buildable.BuildableGameObject.GetComponent<BuildingBase>();
                if (building != null)
                {
                    building.InitializeBuilding(buildable);
                }
            }
            buildablesDictionary.Add(coords, buildable);
            ConstructionLayerManager.Instance.AddToBuildablesSaveData(coords, buildable, this);
            newBuildable = buildable;
        }

        public void DestroyTile(Vector3 worldCoords)
        {
            var coords = TileMap.WorldToCell(worldCoords);
           
            if (!buildablesDictionary.ContainsKey(coords))
            {
                TileMap.SetTile(coords, null);
                return;
            }
            var buildable = buildablesDictionary[coords];
               
            if (buildable == null) return;
           
            buildablesDictionary.Remove(coords);
            ConstructionLayerManager.Instance.RemoveFromSaveData(coords,this);
    
            buildable.Destroy();
        }

        public bool IsEmpty(Vector3 worldCoords, RectInt collisionSpace = default)
        {
            var coords = TileMap.WorldToCell(worldCoords);

            return !buildablesDictionary.ContainsKey(coords) && TileMap.GetTile(coords) == null;
           
        }

    }

}
