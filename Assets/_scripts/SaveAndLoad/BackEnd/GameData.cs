using Callendar;
using GameSystems.BuildingSystem;
using GameSystems.Inventory;
using GameSystems.ShopSystem;
using GameSystems.WaetherSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace GameSystems.SaveLoad
{
    [Serializable]
    public class GameData
    {
        public string Name;
        public string CurrentLevelName;
        public GameMetaData metaData;
        public TimeSaveData timeSaveData;
        public PlayerSaveData playerSaveData;
        //public LevelSaveData levelSaveData;
        //public List<ConstructionLayerSavedata> constructionlayerSaveData = new List<ConstructionLayerSavedata>();
        public WeatherSaveData weatherSaveData;
        public List<InventorySavaData> inventorySavaDatas = new List<InventorySavaData>();
        public BuildingsSaveData buildingsSaveData;
        public DroppedItemSaveData droppedItemSaveData;
        public List<InventorySavaData> shopSystemSaveData = new List<InventorySavaData>();
    }

    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }

    public interface IBind<TData> where TData : ISaveable
    {
        SerializableGuid Id { get; set; }
        void Bind(TData data);
        virtual void Save(ref TData data)
        {
        }
    }

    [Serializable]
    public class GameMetaData
    {
        public string SaveName;
        public int Seed;
        public string CurrentLevelName;
    }

 
}

