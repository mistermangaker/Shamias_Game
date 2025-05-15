using GameSystems.BuildingSystem;
using GameSystems.SaveLoad;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.BuildingSystem
{
    [Serializable]
    public struct BuidablesSaveInformation
    {
        [SerializeField] public BuildingLayer Layer;
        [SerializeField] public Vector3Int position;
        [SerializeField] public string buildableTileType;
        [SerializeField] public BuildingType type;
        [field: SerializeField] public SerializableGuid Id { get; set; }

        public BuidablesSaveInformation(Vector3Int position, string buildableTileType, SerializableGuid id, BuildingLayer layer, BuildingType type = BuildingType.Spawned)
        {
            this.Layer = layer;
            this.position = position;
            this.buildableTileType = buildableTileType;
            this.type = type;
            Id = id;
        }


    }

}