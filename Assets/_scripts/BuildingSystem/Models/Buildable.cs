using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace GameSystems.BuildingSystem
{
    [Serializable]
    public class Buildable
    {
        //[field: SerializeField] public string ItemIdentifierName { get; private set; }

        [field: SerializeField] public Tilemap ParentTileMap {  get; private set; }
        [field: SerializeField] public BuildableTiles BuildableData { get; private set; }
        [field: SerializeField] public GameObject BuildableGameObject { get; private set; }

        [field: SerializeField] public Vector3Int Coordinates { get; private set; }

        [SerializeField] public Vector3 CooridinatesInWorldSpace => ParentTileMap.CellToWorld(Coordinates) + ParentTileMap.cellSize / 2 + (Vector3)BuildableData.TileOffset;

        public BuildingType buildingSpawnType { get; private set; }

        public BuildingLayer buildingLayer { get; private set; }

        public void SetBuildingLayer(BuildingLayer layer)
        {
            buildingLayer = layer;
        }

        public Buildable(BuildableTiles buildableType, Vector3Int coordinates, Tilemap parentTileMap, GameObject buildableGameObject = null, BuildingType buildingType = BuildingType.Spawned, BuildingLayer layer = BuildingLayer.OnGround)
        {
            this.BuildableData = buildableType;
            this.Coordinates = coordinates;
            this.ParentTileMap = parentTileMap;
            this.BuildableGameObject = buildableGameObject;
            this.buildingLayer = layer;
            this.buildingSpawnType = buildingType;
        }
        public void RemoveInConstructionTile()
        {
            ParentTileMap.SetTile(Coordinates, null);
        }
        public SerializableGuid GetOrAddGameObjectSerializableGuid()
        {
            SerializableGuid id = default;
            if(BuildableGameObject != null)
            {
                id = BuildableGameObject.GetOrAddComponent<BuildingSerializableGUID>().Id;
            }
            return id;
        }
        public void InitializeNewBuildableGameObjectSerializableGuid()
        {
            if (BuildableGameObject != null)
            {
                BuildingSerializableGUID data = BuildableGameObject.GetOrAddComponent<BuildingSerializableGUID>();
                data.SetNewId();
            }
        }

        public void SetOrAddGameObjectSerializableGuid(SerializableGuid id)
        {
            if (BuildableGameObject != null)
            {
                BuildingSerializableGUID data = BuildableGameObject.GetOrAddComponent<BuildingSerializableGUID>();
                data.Id = id;
            }
        }
        
        public void Destroy()
        {
            if (BuildableGameObject != null)
            {
                UnityEngine.Object.Destroy(BuildableGameObject);
            }
            ParentTileMap.SetTile(Coordinates, null);
        }

       
    }

}
