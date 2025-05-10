
using GameSystems.SaveLoad;
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameSystems.BuildingSystem
{

    public enum BuildingLayer
    {
        OnGround,
        AboveGround
    }
    [Serializable]
    public struct BuildablesDictionaryInformation
    {
        [SerializeField] public BuildingLayer Layer;
        [SerializeField] public Vector3Int position;
        [SerializeField] public string buildableTileType;
       
  
        [field: SerializeField] public SerializableGuid Id { get; set; }

        public BuildablesDictionaryInformation(Vector3Int position, string buildableTileType, SerializableGuid id, BuildingLayer layer)
        {
            this.Layer = layer;
            this.position = position;
            this.buildableTileType = buildableTileType;
            Id = id;
        }

        
    }
    [Serializable]
    public class BuildingsSaveData : ISaveable
    {
        [field: SerializeField] public SerializableGuid Id { get; set; }
        public List<BuildablesDictionaryInformation> BuildablesData = new List<BuildablesDictionaryInformation>();
       


        public bool Contains(Buildable tile)
        {
            foreach (var data in BuildablesData)
            {
                if (data.buildableTileType == tile.BuildableType.TileName)
                {
                    return true;
                }
            }
            return false;
        }
        public bool Contains(Vector3Int coords)
        {
            foreach (var data in BuildablesData)
            {
                if (data.position == coords)
                {
                    return true;
                }
            }
            return false;
        }
        public void Add(Buildable buildable, BuildingLayer layer)
        {
            BuildablesData.Add(new BuildablesDictionaryInformation(buildable.Coordinates, buildable.BuildableType.TileName, buildable.GetOrAddGameObjectSerializableGuid(), layer));
        }
        public void Remove(Vector3Int coords)
        {
            foreach (var data in BuildablesData)
            {
                if (data.position == coords)
                {
                    BuildablesData.Remove(data);
                    return;
                }
            }
        }
       

    }
    public class ConstructionLayerManager : MonoBehaviour , IBind<BuildingsSaveData>
    {
       
        public static ConstructionLayerManager Instance { get; private set; }

        private Dictionary<Vector3Int, Buildable> buildablesDictionary = new Dictionary<Vector3Int, Buildable>();
        [SerializeField] private BuildingsSaveData buildingsSaveData = new BuildingsSaveData();
       
        
        public Dictionary<Vector3Int, Buildable> GetBuildablesDictionary()
        {
            return buildablesDictionary;
        }

        [SerializeField] private Tilemap TileMap;
        [field: SerializeField] public BuildableTiles ActiveBuildable { get; private set; }
        [SerializeField] private float _maxPlacingDistance = 3f;
        [SerializeField] private float _minPlacingDistance = 0.5f;
        [SerializeField] private MaskLayer BuildiableLayerMask;
        [SerializeField] private ConstructionLayer _floorConstructionLayer;
        [SerializeField] private ConstructionLayer _buildingsConstructionLayer;
        [SerializeField] private ConstructionLayer _tillingConstructionLayer;
        [SerializeField] private MaskLayer tillableLayerMask;
        [SerializeField] private CollisionLayer collisionLayer;
        [SerializeField] private PreviewLayer _previewLayer;
        [SerializeField] private BuildableTiles _tilingTilePrefab;
        

        private PlayerInputManager _playerInputManager => PlayerInputManager.Instance;
        private PlayerController _playerController => PlayerController.Instance;

        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        private void Awake()
        {
            Instance = this;
        }

        public bool TryTilGround(Vector3 position)
        {
            bool canTil = _buildingsConstructionLayer.IsEmpty(position) && _floorConstructionLayer.IsEmpty(position) && _tillingConstructionLayer.IsEmpty(position) && tillableLayerMask.HasTileAtPosition(position);

            if (canTil && IsMouseWithinBuildableRange())
            {
                _tillingConstructionLayer.BuildTile(position, _tilingTilePrefab);
                return true;
            }
            return false;
        }

        public bool TryBuildBuidling(Vector3 position, BuildableTiles buildable)
        {
            //Debug.Log(position);
            //Debug.Log(buildable.BuildableGameObject.name);
            //Debug.Log("tryig to build");
            //var rectInt = buildable.UseCustomCollisionSpace ? buildable.CollisionSpace : default;
            var constructionLayer = buildable.IsFloorTile ? _floorConstructionLayer : _buildingsConstructionLayer;

            //bool tileIsEmpty = AllRelivateTileMapsAreEmptyATCoords(position, rectInt);
            
            if (SpotIsValidForBuidling(position, buildable))
            {
                
                constructionLayer.BuildTile(position, buildable);
                return true;
            }
            return false;
        }

        public void RemoveBuilding(Vector3 posotion)
        {
            if (!_floorConstructionLayer.IsEmpty(posotion)) _floorConstructionLayer.DestroyTile(posotion);
            if (!_buildingsConstructionLayer.IsEmpty(posotion)) _buildingsConstructionLayer.DestroyTile(posotion);
        }
      
       

        private void Update()
        {
            ActiveBuildable = PlayerController.Instance.BuildableForVisuals;
            if (!IsMouseWithinBuildableRange() || _buildingsConstructionLayer == null || _floorConstructionLayer == null || ActiveBuildable == null || _playerInputManager == null)
            {
                _previewLayer.ClearPreview();
                return;
            }
            HandlePreviewImage();
            //var coords = _playerInputManager.MouseToGroundPlane;

            //if (_playerInputManager.IsMouseButtonPressed(MouseButton.Right))
            //{
            //    RemoveBuilding(coords);
            //}

            //if (ActiveBuildable == null)
            //{
            //    _previewLayer.ClearPreview();
            //    return;
            //}
            //var rectInt = ActiveBuildable.UseCustomCollisionSpace ? ActiveBuildable.CollisionSpace : default;

            //bool tileIsEmpty = AllRelivateTileMapsAreEmptyATCoords(coords, rectInt) && !IsMousePositionInsidePlayer();
            //_previewLayer.ShowPreview(ActiveBuildable, coords, tileIsEmpty);
            //if (_playerInputManager.IsMouseButtonPressed(MouseButton.Left) && tileIsEmpty)
            //{
            //    TryBuildBuidling(coords, ActiveBuildable);
            //}
        }

        private void HandlePreviewImage()
        {
            var coords = _playerInputManager.MouseToGroundPlane;
            _previewLayer.ShowPreview(ActiveBuildable, coords, SpotIsValidForBuidling(coords, ActiveBuildable));

        }

        private bool SpotIsValidForBuidling(Vector3 position, BuildableTiles buildable)
        {
            var rectInt = buildable.UseCustomCollisionSpace ? ActiveBuildable.CollisionSpace : default;
            bool tileIsEmpty = (buildable.IsFloorTile) ? IsMouseWithinBuildableRange()&& AllRelivateTileMapsAreEmptyATCoords(position, rectInt) : IsMouseWithinBuildableRange()&& AllRelivateTileMapsAreEmptyATCoords(position, rectInt) && !IsMousePositionInsidePlayer();
            return tileIsEmpty;
        }

        public void AddToBuildablesDictionary(Vector3Int coords, Buildable builadable, ConstructionLayer layer)
        {
            BuildingLayer buildingLayer = layer == _floorConstructionLayer ? BuildingLayer.OnGround : BuildingLayer.AboveGround;
            buildingsSaveData.Add(builadable, buildingLayer);
            buildablesDictionary.Add(coords, builadable);
        }
        public void RemoveFromBuildablesList(Vector3Int coords)
        {
            buildingsSaveData.Remove(coords);
            buildablesDictionary.Remove(coords);
        }

        public bool ContainsCoords(Vector3Int coords)
        {
            return buildablesDictionary.ContainsKey(coords);
        }

        public Buildable GetBuildableFromCoords(Vector3Int coords)
        {
            return buildablesDictionary[coords];
        }
        public void SetCollisions(Buildable builadable, bool value)
        {
            collisionLayer.SetCollisions(builadable, value);
        }


        
        private bool AllRelivateTileMapsAreEmptyATCoords(Vector3 coords, RectInt rectint = default)
        {
            return _buildingsConstructionLayer.IsEmpty(coords, rectint) && _floorConstructionLayer.IsEmpty(coords, rectint)&& _tillingConstructionLayer.IsEmpty(coords) && BuildiableLayerMask.HasTileAtPosition(coords);
        }

        private bool IsMouseWithinBuildableRange()
        {
            return Vector3.Distance(_playerInputManager.MouseToGroundPlane, _playerController.transform.position) <= _maxPlacingDistance;
          
        }
        private bool IsMousePositionInsidePlayer()
        {
            return Vector3.Distance(_playerInputManager.MouseToGroundPlane, _playerController.transform.position) <= _minPlacingDistance;
        }

        public void SetActiveBuildable(BuildableTiles newBuildable)
        {
            ActiveBuildable = newBuildable;
        }

        public void Bind(BuildingsSaveData data)
        {
            foreach (var position in buildablesDictionary)
            {
                _floorConstructionLayer.DestroyTile(position.Key);
                _buildingsConstructionLayer.DestroyTile(position.Key);
            }
            foreach (var tileSaveData in data.BuildablesData)
            {
                BuildableTiles tile = DataBaseManager.Instance.BuildablesDataBase.GetBuildable(tileSaveData.buildableTileType);
                if (tile != null)
                {
                    BuildingLayer layer = tileSaveData.Layer;
                    ConstructionLayer constructionLayer = (layer == BuildingLayer.OnGround)? _floorConstructionLayer: _buildingsConstructionLayer;
                    Vector3 newpos = TileMap.CellToWorld(tileSaveData.position);
                    constructionLayer.BuildTile(newpos, tile, out Buildable buildable);
                    buildable.SetOrAddGameObjectSerializableGuid(tileSaveData.Id);
                }
            }
            buildingsSaveData = data;
            data.Id = Id;
        }

    }
}


