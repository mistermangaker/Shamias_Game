using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameSystems.BuildingSystem
{
    public class BuildingPlacer : MonoBehaviour
    {
        [field:SerializeField] public BuildableTiles ActiveBuildable {  get;  private set; }


        [SerializeField] private float _maxPlacingDistance = 3f;
        //[SerializeField] private float _minPlacingDistance = 1f;

        [SerializeField] private ConstructionLayer _buildingsConstructionLayer;
        [SerializeField] private ConstructionLayer _floorConstructionLayer;

        [SerializeField] private PreviewLayer _previewLayer;

        // [SerializeField] private MouseUser mouseUser;
        [SerializeField] private PlayerInputManager _playerInputManager;


        private void Start()
        {
            _playerInputManager = PlayerInputManager.Instance;
        }
        //todo rework this to use the new item use system
        //todo rework this entirely
        private void Update()
        {
            if(_playerInputManager == null) return;
            if (!IsMouseWithinBuildableRange() || _buildingsConstructionLayer == null || _floorConstructionLayer == null)
            {
                _previewLayer.ClearPreview();
                return;
            }
           
            var coords = _playerInputManager.MouseInWorldPoint;
            
            if (_playerInputManager.IsMouseButtonPressed(MouseButton.Right))
            {
                if(!_floorConstructionLayer.IsEmpty(coords)) _floorConstructionLayer.DestroyTile(coords);
                if(!_buildingsConstructionLayer.IsEmpty(coords)) _buildingsConstructionLayer.DestroyTile(coords);
            }

            if (ActiveBuildable == null) return;

            var rectInt = ActiveBuildable.UseCustomCollisionSpace ? ActiveBuildable.CollisionSpace : default;
            var constructionLayer = ActiveBuildable.IsFloorTile? _floorConstructionLayer : _buildingsConstructionLayer;
            
            bool tileIsEmpty = AllRelivateTileMapsAreEmptyATCoords(coords, rectInt);
            _previewLayer.ShowPreview(ActiveBuildable, coords, tileIsEmpty);
            if(_playerInputManager.IsMouseButtonPressed(MouseButton.Left) && tileIsEmpty)
            {
                constructionLayer.BuildTile(coords, ActiveBuildable);
            }
        }

        private bool AllRelivateTileMapsAreEmptyATCoords(Vector3 coords, RectInt rectint = default)
        {
            return _buildingsConstructionLayer.IsEmpty(coords, rectint) && _floorConstructionLayer.IsEmpty(coords, rectint);
        }

        private bool IsMouseWithinBuildableRange()
        {
            return Vector3.Distance(_playerInputManager.MouseInWorldPoint, transform.position) <= _maxPlacingDistance;
        }

        public void SetActiveBuildable(BuildableTiles newBuildable)
        {
            ActiveBuildable = newBuildable;
        }

    }

}
