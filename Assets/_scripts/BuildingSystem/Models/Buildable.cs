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
        [field: SerializeField] public BuildableTiles BuildableType { get; private set; }
        [field: SerializeField] public GameObject BuildableGameObject { get; private set; }

        [field: SerializeField] public Vector3Int Coordinates { get; private set; }

        [field: SerializeField] public TileBase InConstructionTile { get; private set; }
        [SerializeField] public Vector3 CooridinatesInWorldSpace => ParentTileMap.CellToWorld(Coordinates) + ParentTileMap.cellSize / 2 + (Vector3)BuildableType.TileOffset;



        public Buildable(BuildableTiles buildableType, Vector3Int coordinates, Tilemap parentTileMap, GameObject buildableGameObject = null, TileBase inConstructionTile = null)
        {
            this.BuildableType = buildableType;
            this.Coordinates = coordinates;
            this.ParentTileMap = parentTileMap;
            this.BuildableGameObject = buildableGameObject;
            this.InConstructionTile = inConstructionTile;
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
                id = BuildableGameObject.GetOrAddComponent<BuildingSaveData>().Id;
            }
            return id;
        }
        public void InitializeNewBuildableGameObjectSerializableGuid()
        {
            if (BuildableGameObject != null)
            {
                BuildingSaveData data = BuildableGameObject.GetOrAddComponent<BuildingSaveData>();
                data.SetNewId();
            }
        }

        public void SetOrAddGameObjectSerializableGuid(SerializableGuid id)
        {
            if (BuildableGameObject != null)
            {
                BuildingSaveData data = BuildableGameObject.GetOrAddComponent<BuildingSaveData>();
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

        public void IterateCollisionSpace(RectIntExtensions.RectAction action)
        {
            BuildableType.CollisionSpace.Iterate(Coordinates, action);
        }

        public bool IterateCollisionSpace(RectIntExtensions.RectActionBool action)
        {
             return BuildableType.CollisionSpace.Iterate(Coordinates, action);
        }
    }

}
