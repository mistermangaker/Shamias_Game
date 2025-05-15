using Callendar;
using System.Collections.Generic;
using UnityEngine;



public class ForageableSpawner : MonoBehaviour
{
    [SerializeField] private List<BuildableTiles> SpringForageAblesToSpawn;
    [SerializeField] private List<BuildableTiles> folliageToSpawn;

    [SerializeField] private List<BoundingBoxes> SpawnZones;

    [SerializeField] private int amountToSpawn;
    private int numberSpawned;

  
    public BoundingBoxes GetRandomBoundingBox()
    {
        return SpawnZones[Random.Range(0, SpawnZones.Count)];
    }

    public BuildableTiles GetRandomInSeasonForagable()
    {
        return SpringForageAblesToSpawn[Random.Range(0, SpringForageAblesToSpawn.Count)];
    }

    public BuildableTiles GetRandomFolliage()
    {
        return folliageToSpawn[Random.Range(0, folliageToSpawn.Count)];
    }

    private void OnEnable()
    {
        TimeManager.OnNewWeek += HandleSpawning;
        TimeManager.OnNewSeason += HandleNewSeason;
    }

    private void OnDisable()
    {
        TimeManager.OnNewWeek -= HandleSpawning;
        TimeManager.OnNewSeason -= HandleNewSeason;
    }


    private void HandleNewSeason(DateTimeStamp stamp)
    {
        

    }



    private void HandleSpawning(DateTimeStamp stamp)
    {
        if(stamp.Season == Season.Spring)
        {
            if(SpringForageAblesToSpawn ==null || SpawnZones == null) return;
            ClearAllForageables();
            SpawnRandomForageable();
        }
    }

    [ContextMenu("spawn random forageable")]
    private void SpawnRandomForageable()
    {
        while(numberSpawned <= amountToSpawn)
        {
            BuildableTiles tile = GetRandomInSeasonForagable();
            BoundingBoxes pos = GetRandomBoundingBox();
            Vector3 positionToSpawn = pos.GetRandomXZPositionInBoundingBox();
            //Debug.Log($"{tile.TileName} {positionToSpawn}");
            EventBus<OnTrySpawnFolliage>.Raise(new OnTrySpawnFolliage
            {
                spawnPosition = positionToSpawn,
                BuildableTiles = tile,
                BuildingType = GameSystems.BuildingSystem.BuildingType.Temporary,
            });
            numberSpawned++;
        }
        numberSpawned = 0;
    }
    [ContextMenu("spawn random folliage")]
    private void SpawnRandomFolliage()
    {
        while (numberSpawned <= amountToSpawn)
        {
            BuildableTiles tile = GetRandomFolliage();
            BoundingBoxes pos = GetRandomBoundingBox();
            Vector3 positionToSpawn = pos.GetRandomXZPositionInBoundingBox();
            //Debug.Log($"{tile.TileName} {positionToSpawn}");
            EventBus<OnTrySpawnFolliage>.Raise(new OnTrySpawnFolliage
            {
                spawnPosition = positionToSpawn,
                BuildableTiles = tile,
                BuildingType = GameSystems.BuildingSystem.BuildingType.Spawned,
            });
            numberSpawned++;
        }
        numberSpawned = 0;
    }


    [ContextMenu("clear all forageables")]
    private void ClearAllForageables()
    {
        EventBus<ClearSpawnForagables>.Raise(new ClearSpawnForagables());
    }
}
