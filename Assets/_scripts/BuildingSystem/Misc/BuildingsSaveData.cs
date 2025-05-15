
using GameSystems.SaveLoad;
using System;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GameSystems.BuildingSystem
{
    [Serializable]
    public class BuildingsSaveData : ISaveable
    {
        [field: SerializeField] public SerializableGuid Id { get; set; }
        [field: SerializeField] private List<BuidablesSaveInformation> groundBuildablesSaveData = new List<BuidablesSaveInformation>();
        [field: SerializeField] private List<BuidablesSaveInformation> tillingGroundSavedata = new List<BuidablesSaveInformation>();
        [field: SerializeField] private List<BuidablesSaveInformation> AboveGroundBuildablesData = new List<BuidablesSaveInformation>();

        [field: SerializeField] private List<BuidablesSaveInformation> transitoryBuildablesData = new List<BuidablesSaveInformation>();
        [field: SerializeField] private List<BuidablesSaveInformation> _allBuidablesdata = new List<BuidablesSaveInformation>();
        public List<BuidablesSaveInformation> GroundBuildablesSaveDataForReference => groundBuildablesSaveData;
        public List<BuidablesSaveInformation> TillingGroundSavedataForReference => tillingGroundSavedata;
        public List<BuidablesSaveInformation> AboveGroundBuildablesDataForReference => AboveGroundBuildablesData;
        public List<BuidablesSaveInformation> AllBuildingInformationForReference => _allBuidablesdata;
        public List<BuidablesSaveInformation> TransitoryBuildablesData => transitoryBuildablesData;

       
        
        private void RefreshAllBuidablesData()
        {
            _allBuidablesdata.Clear();
            _allBuidablesdata.AddRange(tillingGroundSavedata);
            _allBuidablesdata.AddRange(groundBuildablesSaveData);
            _allBuidablesdata.AddRange(AboveGroundBuildablesData);
            //_allBuidablesdata.AddRange(transitoryBuildablesData);
            
        }
        

        public bool Contains(Buildable tile)
        {
            foreach (var data in _allBuidablesdata)
            {
                if (data.buildableTileType == tile.BuildableData.TileName)
                {
                    return true;
                }
            }
            
            return false;
        }
        public bool Contains(Vector3Int coords)
        {
            foreach (var data in _allBuidablesdata)
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
            BuidablesSaveInformation info = new BuidablesSaveInformation(buildable.Coordinates, buildable.BuildableData.TileName, buildable.GetOrAddGameObjectSerializableGuid(), layer);
            if (layer == BuildingLayer.TillingLayer)
            {
                tillingGroundSavedata.Add(info);
            }
            else if(layer == BuildingLayer.OnGround)
            {
                groundBuildablesSaveData.Add(info);
            }
            else if(layer == BuildingLayer.AboveGround)
            {
                AboveGroundBuildablesData.Add(info);
            }
            if(buildable.buildingSpawnType == BuildingType.Temporary)
            {
                transitoryBuildablesData.Add(info);
            }
            RefreshAllBuidablesData();
        }

        //messy messy
        public void Remove(Vector3Int coords, BuildingLayer layer)
        {
            foreach (var data in transitoryBuildablesData)
            {
                if (data.position == coords)
                {
                    transitoryBuildablesData.Remove(data);
                    RefreshAllBuidablesData();
                    return;
                }
            }
            List<BuidablesSaveInformation> list;
            if (layer == BuildingLayer.TillingLayer)
            {
                list = tillingGroundSavedata;
            }
            else if (layer == BuildingLayer.OnGround)
            {
                list = groundBuildablesSaveData;
            }
            else 
            {
                list = AboveGroundBuildablesData;
            }
            
            foreach (var data in list)
            {
                if (data.position == coords)
                {
                    list.Remove(data);
                    break;
                }
            }
           
            RefreshAllBuidablesData();
            return;
        }
       

    }
}


