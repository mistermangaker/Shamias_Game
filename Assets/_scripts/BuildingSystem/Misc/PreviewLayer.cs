using UnityEngine;

namespace GameSystems.BuildingSystem
{
    public class PreviewLayer : TileMapLayer
    {
        [SerializeField] private SpriteRenderer _previewRenderer;

        public void ShowPreview(BuildableTiles item , Vector3 worldCoords, bool isValid)
        {
            var coords = TileMap.WorldToCell(worldCoords);
            _previewRenderer.enabled = true;
            _previewRenderer.transform.position = TileMap.CellToWorld(coords) + TileMap.cellSize / 2 + (Vector3)item.TileOffset;
            _previewRenderer.sprite = item.DisplaySprite;
            _previewRenderer.color = isValid ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
        }

        public void ClearPreview()
        {
            _previewRenderer.enabled = false;
        }
    }
}

