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
        //[Obsolete]
        //private Dictionary<Vector3Int, Buildable> buildablesDictionary = new Dictionary<Vector3Int, Buildable>(); 

        //[field: SerializeField] private ConstructionLayerSavedata saveData;
        //[Obsolete]
        //[SerializeField] private CollisionLayer collisionLayer;

        //[field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        //public void Bind(ConstructionLayerSavedata data)
        //{
        //    foreach (var position in buildablesDictionary)
        //    {
        //        DestroyTile(position.Key);
        //    }
            
        //    foreach (var tileSaveData in data.buidableTileSaveDatas)
        //    {
        //        BuildableTiles tile = DataBaseManager.Instance.BuildablesDataBase.GetBuildable(tileSaveData.buildableTileType);
        //        if(tile != null)
        //        {
        //            BuildTile((Vector3)tileSaveData.position, tile, out Buildable buildable);
        //            buildable.SetOrAddGameObjectSerializableGuid(tileSaveData.Id);
        //        }
                
        //    }
        //    saveData = data;
        //    data.Id = Id;
        //}

       // private ConstructionLayerManager _constructionLayerManager;
        protected override void Awake()
        {
            base.Awake();
            //saveData = new ConstructionLayerSavedata();
           // buildablesDictionary = GetComponentInParent<ConstructionLayerManager>().GetBuildablesDictionary();
        }

        private void Start()
        {
            //_constructionLayerManager = ConstructionLayerManager.Instance;
        }

        //public void Save(ref ConstructionLayerSavedata data)
        //{
        //    //saveData = data;
        //    //data.Id = Id;
        //}
        public void BuildTile(Vector3 worldCoords, BuildableTiles item)
        {
            BuildTile(worldCoords, item, out Buildable buildable);
        }

        public void BuildTile(Vector3 worldCoords, BuildableTiles item, out Buildable newBuildable)
        {
            GameObject itemObject = null;
            var coords = TileMap.WorldToCell(worldCoords);
            
            
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
            if(item.BuildableGameObject != null)
            {
                itemObject = Instantiate(
                    item.BuildableGameObject,
                    TileMap.CellToWorld(coords) + TileMap.cellSize / 2 + (Vector3)item.TileOffset, Quaternion.identity
                    ) ;
            }
            var buildable = new Buildable(item, coords, TileMap, itemObject);
            if (buildable.BuildableGameObject != null)
            {
                buildable.InitializeNewBuildableGameObjectSerializableGuid();
                BuildingBase building = buildable.BuildableGameObject.GetComponent<BuildingBase>();
                if (building != null)
                {
                    building.InitializeBuilding(buildable);
                }
                //PlantBase plant = buildable.BuildableGameObject.GetComponent<PlantBase>();
                //if (plant != null)
                //{
                //    plant.SetUpPlant(buildable);
                //}

            }
            if(item.UseCustomCollisionSpace)
            {
                ConstructionLayerManager.Instance.SetCollisions(buildable, true);
                //collisionLayer.SetCollisions(buildable, true);
                RegisterBuildableCollisionSpace(buildable);
            }
            else
            {
                ConstructionLayerManager.Instance.AddToBuildablesDictionary(coords, buildable, this);
                //buildablesDictionary.Add(coords, buildable);
            }
            
            //saveData.AddBuildable(buildable);
            newBuildable = buildable;

        }

        public void DestroyTile(Vector3 worldCoords)
        {
            var coords = TileMap.WorldToCell(worldCoords);
            if (!ConstructionLayerManager.Instance.ContainsCoords(coords))
            {
                TileMap.SetTile(coords, null);
                return;
            }
            var buildable = ConstructionLayerManager.Instance.GetBuildableFromCoords(coords);
            if (buildable.BuildableType.UseCustomCollisionSpace)
            {
                ConstructionLayerManager.Instance.SetCollisions(buildable, false);
                //collisionLayer.SetCollisions(buildable, false);
                UnRegisterBuildableCollisionSpace(buildable);
            }
            ConstructionLayerManager.Instance.RemoveFromBuildablesList(coords);
    
            //if (!buildablesDictionary.ContainsKey(coords)) 
            //{ 
            //    TileMap.SetTile(coords, null);
            //    return; 
            //} 
            //var buildable = buildablesDictionary[coords];
            //if (buildable.BuildableType.UseCustomCollisionSpace)
            //{
            //    collisionLayer.SetCollisions(buildable, false);
            //    UnRegisterBuildableCollisionSpace(buildable);
            //}
            //buildablesDictionary.Remove(coords);
            buildable.Destroy();
        }

        public bool IsEmpty(Vector3 worldCoords, RectInt collisionSpace = default)
        {
            var coords = TileMap.WorldToCell(worldCoords);

            if (!collisionSpace.Equals(default))
            {
                return !IsRectOccupied(coords, collisionSpace);
            }

            return !ConstructionLayerManager.Instance.ContainsCoords(coords) && TileMap.GetTile(coords) == null;
        }

        private bool IsRectOccupied(Vector3Int coords, RectInt rect)
        {
            return rect.Iterate(coords, tileCoords => ConstructionLayerManager.Instance.ContainsCoords(tileCoords));
        }

        private void RegisterBuildableCollisionSpace(Buildable buildable)
        {
            buildable.IterateCollisionSpace(tilecoords => ConstructionLayerManager.Instance.AddToBuildablesDictionary(tilecoords, buildable, this));
        }

        private void UnRegisterBuildableCollisionSpace(Buildable buildable)
        {
            buildable.IterateCollisionSpace(tilecoords => {
                ConstructionLayerManager.Instance.RemoveFromBuildablesList(tilecoords);
                //buildablesDictionary.Remove(tilecoords);
            });
        }

    }

}
