using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "BuildableTiles", menuName = "Building System/BuildableTiles")]
public class BuildableTiles : ScriptableObject
{
    [field:SerializeField] public string TileName {  get; private set; }
    [field:SerializeField] public TileBase TileToPlace {  get; private set; }
    [field:SerializeField] public bool IsFloorTile { get; private set; }
    [field:SerializeField] public Sprite DisplaySprite { get; private set; }
    [field:SerializeField] public Vector2 TileOffset {  get; private set; }
    [field:SerializeField] public GameObject BuildableGameObject { get; private set; }
    [field:SerializeField] public bool UseCustomCollisionSpace { get; private set; }
    [field:SerializeField] public RectInt CollisionSpace { get; private set; }
    [field: SerializeField] public GrowthStages GrowthStage { get; private set; }
   
   
}
