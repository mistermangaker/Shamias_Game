using GameSystems.BuildingSystem;
using GameSystems.SaveLoad;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConstructionLayerSavedata : ISaveable
{
    [field: SerializeField] public SerializableGuid Id { get; set; }
    public List<BuidableTileSaveData> buidableTileSaveDatas = new List<BuidableTileSaveData>();

    public void AddBuildable(Buildable buildable)
    {
        buidableTileSaveDatas.Add(new BuidableTileSaveData(buildable.Coordinates, buildable.BuildableType.TileName, buildable.GetOrAddGameObjectSerializableGuid()));
    }
    
}

[Serializable]
public class BuidableTileSaveData 
{
    [SerializeField] public Vector3Int position;
    [SerializeField] public string buildableTileType;
    [field: SerializeField] public SerializableGuid Id { get; set; }

    public BuidableTileSaveData(Vector3Int position, string buildableTileType, SerializableGuid id)
    {
        this.position = position;
        this.buildableTileType = buildableTileType;
        Id = id;
    }
}