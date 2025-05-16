using Callendar;
using GameSystems.BuildingSystem;
using GameSystems.Inventory;
using GameSystems.ShopSystem;
using GameSystems.WaetherSystem;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSystems.SaveLoad
{
    public struct GameSaveUIInformation
    {
        public string Name;
        public string dateLastPlayed;
        public DateTimeStamp currentGameDate;
    }


    public class SaveLoadSystem : MonoBehaviour
    {
        public static SaveLoadSystem Instance;
        private bool issaving;

        [SerializeField] public GameData gameData;

        IDataService dataService;

        private void Awake()
        {
            if (Instance != null )
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            dataService = new FileDataService(new JsonSerializer());
        }

       
        private void OnEnable() => SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        private void OnDisable() => SceneManager.sceneLoaded -= SceneManager_sceneLoaded;

       
        // this method fires with every new scene loaded;
        // todo seperate scene amanagement from the save feature
        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            GenerateNewGameStat(gameData);
           
            BindInformation();
        }

        private void BindInformation()
        {
           
            Bind<TimeManager, TimeSaveData>(gameData.timeSaveData);
            Bind<PlayerController, PlayerSaveData>(gameData.playerSaveData);
           
            Bind<ConstructionLayerManager, BuildingsSaveData>(gameData.buildingsSaveData);
            Bind<WeatherManager, WeatherSaveData>(gameData.weatherSaveData);    
            Bind<InventoryHolder, InventorySavaData>(gameData.inventorySavaDatas);
            Bind<OnGroundItemManager, DroppedItemSaveData>(gameData.droppedItemSaveData);
            Bind<ShopKeeper, InventorySavaData>(gameData.shopSystemSaveData);
        }


        void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
            if (entity != null)
            {
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                }
                entity.Bind(data);
            }
        }

        void Bind<T, TData>(List<TData> datas) where T: MonoBehaviour, IBind<TData> where TData: ISaveable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);
            foreach ( var entity in entities)
            {
                var data = datas.FirstOrDefault(d => d.Id == entity.Id);
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                    datas.Add(data);
                }
                entity.Bind(data);
            }
        }
        public string GetCurrentSaveName()
        {
            return gameData.Name;
        }


        public bool MakeNewGameSave(string newname)
        {
            if (dataService.ListSaves().ToList().Contains(newname))
            {
                List<string> newsaves = dataService.ListSaves().ToList().FindAll(i => i.StartsWith(newname));
                string newNewName = $"{newname}_({newsaves.Count})";
                gameData.Name = newname;
                SaveGame(newNewName);
                return true;
            }
            gameData.Name = newname;
            SaveGame();
            return true;
            
        }

        

        public void NewGame()
        {
            gameData = new GameData
            {
                Name = "New Game",
                CurrentLevelName = "GameScene"
            };
            GenerateNewGameStat(gameData);
            SceneManager.LoadScene(gameData.CurrentLevelName);
           
            
        }
        private void GenerateNewGameStat(GameData data)
        {
            if (data.metaData.Seed != 0) return;
            int gameseed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

            GameMetaData metaData = new GameMetaData
            {
                SaveName = "New Game",
                Seed = gameseed,
                CurrentLevelName = "GameScene"
            };
            data.metaData = metaData;
            Debug.Log("started new game");
            EventBus<OnGameStart>.Raise(new OnGameStart());
            Debug.Log("guh");
        }

        public void SaveGame()
        {
            SaveGame(gameData.Name);
        }
        public void SaveGame(string newname)
        {
            if(issaving)return;
            issaving = true;
            SaveSavables();
            dataService.Save(gameData, true, newname);
            Debug.Log("game saved");
            issaving = false;
        }

        private void SaveSavables()
        {
            Save<TimeManager, TimeSaveData>(ref gameData.timeSaveData);
            Save<PlayerController, PlayerSaveData>(ref gameData.playerSaveData);
           
            Save<ConstructionLayerManager, BuildingsSaveData>(ref gameData.buildingsSaveData);
            Save<WeatherManager, WeatherSaveData>(ref gameData.weatherSaveData);
            Save<InventoryHolder, InventorySavaData>(ref gameData.inventorySavaDatas);
            Save<OnGroundItemManager, DroppedItemSaveData>(ref gameData.droppedItemSaveData);
            Save<ShopKeeper, InventorySavaData>(ref gameData.shopSystemSaveData);
        }
        void Save<T, TData>(ref TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
            if (entity != null)
            {
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                }
                entity.Save(ref data);
            }
        }


        void Save<T, TData>(ref List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);
            foreach (var entity in entities)
            {
                var data = datas.FirstOrDefault(d => d.Id == entity.Id);
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                    datas.Add(data);
                }
                entity.Save(ref data);
            }
        }

        public void LoadGame(string name) 
        {
            gameData = dataService.Load(name);
            if (string.IsNullOrWhiteSpace(gameData.CurrentLevelName))
            {
                gameData.CurrentLevelName = "GameScene";
            }
            Debug.Log("game loaded");
            SceneManager.LoadScene(gameData.CurrentLevelName);
        }
        public void DeleteGame(string gameName) => dataService.Delete(gameName);
        public void ReloadGame() => LoadGame(gameData.Name);
        
        public List<GameSaveUIInformation> GetSaveData()
        {
            List<GameSaveUIInformation> list = new List<GameSaveUIInformation>();
            foreach (var save in dataService.ListSaves().ToList())
            {
                list.Add(new GameSaveUIInformation {
                    Name = save 
                });
            }
            
            return list;
        }


    }

}
