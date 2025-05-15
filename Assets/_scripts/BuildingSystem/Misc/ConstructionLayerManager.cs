

using GameSystems.SaveLoad;
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace GameSystems.BuildingSystem
{

    public class ConstructionLayerManager : MonoBehaviour , IBind<BuildingsSaveData>
    {
       
        public static ConstructionLayerManager Instance { get; private set; }

        //private Dictionary<Vector3Int, Buildable> buildablesDictionaryTillingLayer = new Dictionary<Vector3Int, Buildable>();
        //private Dictionary<Vector3Int, Buildable> buildablesDictionaryGroundLayer = new Dictionary<Vector3Int, Buildable>();
        //private Dictionary<Vector3Int, Buildable> buildablesDictionaryAboveGroundLayer = new Dictionary<Vector3Int, Buildable>();

        [SerializeField] private BuildingsSaveData buildingsSaveData = new BuildingsSaveData();
       
        
        

        [SerializeField] private Tilemap TileMap;
        [field: SerializeField] public BuildableTiles ActiveBuildable { get; private set; }
        [SerializeField] private float _maxPlacingDistance = 3f;
        [SerializeField] private float _minPlacingDistance = 0.5f;
        [SerializeField] private MaskLayer BuildiableLayerMask;
        [SerializeField] private ConstructionLayer _floorConstructionLayer;
        [SerializeField] private ConstructionLayer _buildingsConstructionLayer;
        [SerializeField] private ConstructionLayer _tillingConstructionLayer;
        [SerializeField] private MaskLayer tillableLayerMask;
        [SerializeField] private PreviewLayer _previewLayer;
        [SerializeField] private BuildableTiles _tilingTilePrefab;
        

        private PlayerInputManager _playerInputManager => PlayerInputManager.Instance;
        private PlayerController _playerController => PlayerController.Instance;

        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();

        EventBinding<OnBuildingRemovalRequested> buildingRemovalRequested;
        EventBinding<OnPlayerEquipedItemChanged> playerItemChanged;
        private EventBinding<OnTrySpawnFolliage> trySpawnForageables;
        private EventBinding<ClearSpawnForagables> clearForageables;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            clearForageables = new EventBinding<ClearSpawnForagables>(ClearForagables);
            EventBus<ClearSpawnForagables>.Register(clearForageables);
            trySpawnForageables = new EventBinding<OnTrySpawnFolliage>(HandleSpawningForagables);
            EventBus<OnTrySpawnFolliage>.Register(trySpawnForageables);
            buildingRemovalRequested = new EventBinding<OnBuildingRemovalRequested>(HandleBuildingRemoval);
            EventBus<OnBuildingRemovalRequested>.Register(buildingRemovalRequested);
            playerItemChanged = new EventBinding<OnPlayerEquipedItemChanged>(HandleNewItemEquiped);
            EventBus<OnPlayerEquipedItemChanged>.Register(playerItemChanged);
        }
        private void OnDisable()
        {
            EventBus<ClearSpawnForagables>.Deregister(clearForageables);
            EventBus<OnTrySpawnFolliage>.Deregister(trySpawnForageables);
            EventBus<OnBuildingRemovalRequested>.Deregister(buildingRemovalRequested);
            EventBus<OnPlayerEquipedItemChanged>.Deregister(playerItemChanged);
        }

        private void ClearForagables(ClearSpawnForagables foragables)
        {
            List<Vector3Int> list = new List<Vector3Int>();
            foreach(var item in buildingsSaveData.AllBuildingInformationForReference) 
            {
                if(item.type == BuildingType.Temporary)
                {
                    list.Add(item.position);
                }
            }

            foreach(var item in list)
            {
                Vector3 newpos = TileMap.CellToWorld(item);
                RemoveBuilding(newpos);
            }
        }
        private void HandleSpawningForagables(OnTrySpawnFolliage foragable)
        {
            if(foragable.BuildableTiles != null)
            {
                if (AllRelivateTileMapsAreEmptyATCoords(foragable.spawnPosition))
                {
                    BuildingSuggestion buildingSuggestion = new BuildingSuggestion(foragable.spawnPosition, foragable.BuildableTiles, foragable.BuildingType);
                    BuildTile(buildingSuggestion);
                }
            }
        }
        private void HandleBuildingRemoval(OnBuildingRemovalRequested building)
        {
            RemoveBuilding(building.position);
        }
        private void HandleNewItemEquiped(OnPlayerEquipedItemChanged item)
        {
            ActiveBuildable = item.Item.GameItemData?.Buildable;
        }

        public bool TryTilGround(Vector3 position)
        {
            bool canTil = _buildingsConstructionLayer.IsEmpty(position) && _floorConstructionLayer.IsEmpty(position) &&  tillableLayerMask.HasTileAtPosition(position);
            bool _tillingAreaIsEmpty = _tillingConstructionLayer.IsEmpty(position);
  
            if (canTil && _tillingAreaIsEmpty)
            {
                BuildingSuggestion buildingSuggestion = new BuildingSuggestion(position, _tilingTilePrefab,BuildingType.PlayerPlaced);
                _tillingConstructionLayer.BuildTile(buildingSuggestion);
                return true;
            }
            else if(!_tillingAreaIsEmpty)
            {
                _tillingConstructionLayer.DestroyTile(position);
                return true;
            }
            return false;
        }

        public bool TryBuildBuidling(Vector3 position, BuildableTiles buildable)
        {
            if (SpotIsValidForBuidling(position, buildable))
            {
                BuildingSuggestion buildingSuggestion = new BuildingSuggestion(position, buildable, BuildingType.PlayerPlaced);
                BuildTile(buildingSuggestion);
                return true;
            }
            return false;
        }

        public void RemoveBuilding(Vector3 position)
        {
            if (!_floorConstructionLayer.IsEmpty(position)) _floorConstructionLayer.DestroyTile(position);
            if (!_buildingsConstructionLayer.IsEmpty(position)) _buildingsConstructionLayer.DestroyTile(position);
            if (!_tillingConstructionLayer.IsEmpty(position)) _tillingConstructionLayer.DestroyTile(position);
        }
        
        private void BuildTile(BuildingSuggestion suggestion)
        {
            var constructionLayer = suggestion.BuildableTiles.IsFloorTile ? _floorConstructionLayer : _buildingsConstructionLayer;
            constructionLayer.BuildTile(suggestion);
        }

        private void Update()
        {
            UpdatePreviewImage();
        }

        private void UpdatePreviewImage()
        {
            if (!IsMouseWithinBuildableRange() || _buildingsConstructionLayer == null || _floorConstructionLayer == null || ActiveBuildable == null || _playerInputManager == null)
            {
                _previewLayer.ClearPreview();
                return;
            }
            var coords = _playerInputManager.MouseToGroundPlane;
            _previewLayer.ShowPreview(ActiveBuildable, coords, SpotIsValidForBuidling(coords, ActiveBuildable));

        }

        private bool SpotIsValidForBuidling(Vector3 position, BuildableTiles buildable)
        {
            if (buildable.IsPlantableOnSoil)return !_tillingConstructionLayer.IsEmpty(position) && IsMouseWithinBuildableRange();

            bool tileIsEmpty = (buildable.IsFloorTile) ? IsMouseWithinBuildableRange() && AllRelivateTileMapsAreEmptyATCoords(position) : IsMouseWithinBuildableRange() && AllRelivateTileMapsAreEmptyATCoords(position) && !IsMousePositionInsidePlayer();
            
            return tileIsEmpty;
        }

        private BuildingLayer GetBuildingLayer(ConstructionLayer layer)
        {
            if (layer == _tillingConstructionLayer) return BuildingLayer.TillingLayer;

            else if (layer == _floorConstructionLayer) return BuildingLayer.OnGround;

            else return BuildingLayer.AboveGround;

        }
        public void AddToBuildablesSaveData(Vector3Int coords, Buildable builadable, ConstructionLayer layer)
        {
            BuildingLayer buildingLayer = GetBuildingLayer(layer);
            builadable.SetBuildingLayer(buildingLayer);
      
            buildingsSaveData.Add(builadable, buildingLayer);
        }
        public void RemoveFromSaveData(Vector3Int coords, ConstructionLayer layer)
        {
            BuildingLayer buildingLayer = GetBuildingLayer(layer);
            buildingsSaveData.Remove(coords, buildingLayer);
        }

        private bool AllRelivateTileMapsAreEmptyATCoords(Vector3 coords, RectInt rectint = default)
        {
            bool emptyAtBuildingLayer = _buildingsConstructionLayer.IsEmpty(coords, rectint);
            bool emptyAtGroundLayer = _floorConstructionLayer.IsEmpty(coords, rectint);
            bool emptyAtTillingLayer = _tillingConstructionLayer.IsEmpty(coords);
            bool NotEmptyAtBuidlinLayerMask = BuildiableLayerMask.HasTileAtPosition(coords);
            return emptyAtBuildingLayer && emptyAtGroundLayer && emptyAtTillingLayer && NotEmptyAtBuidlinLayerMask;
        }

        private bool IsMouseWithinBuildableRange()
        {
            return Vector3.Distance(_playerInputManager.MouseToGroundPlane, _playerController.transform.position) <= _maxPlacingDistance;
        }
        private bool IsMousePositionInsidePlayer()
        {
            return Vector3.Distance(_playerInputManager.MouseToGroundPlane, _playerController.transform.position) <= _minPlacingDistance;
        }


        public void Bind(BuildingsSaveData data)
        {
            foreach (var tileSaveData in data.AllBuildingInformationForReference)
            {
                BuildableTiles tile = DataBase.GetBuildable(tileSaveData.buildableTileType);
                   
                if (tile != null)
                {
                    BuildingLayer layer = tileSaveData.Layer;
                    ConstructionLayer constructionLayer = layer switch
                    {
                        BuildingLayer.TillingLayer => _tillingConstructionLayer,
                        BuildingLayer.OnGround => _floorConstructionLayer,
                        BuildingLayer.AboveGround => _buildingsConstructionLayer,
                        _ => null
                    } ;
                   
                    Vector3 newpos = TileMap.CellToWorld(tileSaveData.position);

                    BuildingSuggestion buildingSuggestion = new BuildingSuggestion(newpos, tile, tileSaveData.type);
                    constructionLayer.BuildTile(buildingSuggestion, out Buildable buildable);
                    buildable.SetOrAddGameObjectSerializableGuid(tileSaveData.Id);
                }
            }
            buildingsSaveData = data;
            data.Id = Id;
        }

    }
}


