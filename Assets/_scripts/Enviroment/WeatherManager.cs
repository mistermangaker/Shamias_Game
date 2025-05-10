using Callendar;
using GameSystems.SaveLoad;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameSystems.WaetherSystem
{
    [Serializable]
    public class WeatherSaveData : ISaveable
    {
        [SerializeField] public SerializableGuid Id { get; set; }
        [SerializeField] public List<Weather> weatherDictionary = new List<Weather>();
        public Weather currentWeather;

    }
    public enum Weather
    {
        Clear,
        Cloudy,
        Raining,
        ThunderStorm
    }
    public class WeatherManager : MonoBehaviour, IBind<WeatherSaveData>
    {

        //public int hoursBetweenNewWeather = 2;

        [field: SerializeField] private static Weather currentWeather = Weather.Clear;
        public static Weather CurrentWeather => currentWeather;

        [field:SerializeField] public SerializableGuid Id { get ; set ; } = SerializableGuid.NewGuid();

        public static UnityAction<Weather> OnWeatherChange;

        [SerializeField] private WeatherSaveData weatherSaveData;

        private void Start()
        {
            TimeManager.OnNewDay += OnNewDay;
            TimeManager.OnNewSeason += OnNewSeason;
            //OnNewSeason(TimeManager.Instance.CurrentGameTime);
        }

        private void OnDisable()
        {
            TimeManager.OnNewDay -= OnNewDay;
            TimeManager.OnNewSeason -= OnNewSeason;

        }
        private void OnNewSeason(DateTimeStamp time)
        {
            weatherSaveData.weatherDictionary.Clear();
            float clearChance = 10f;
            float cloudyChance = 10f;
            float rainyChance = 10f;
            float thunderStormChance = 10f;
            //float tempuratureVarianceFactor = 1.1f;

            switch (time.Season)
            {
                case Season.Spring:
                    clearChance -= 5f;
                    rainyChance += 5f;
                    cloudyChance += 5f;
                    break;
                case Season.Summer:
                    clearChance += 10f;
                    thunderStormChance += 5f;
                    cloudyChance -= 5f;
                    break;
                case Season.Fall:
                    thunderStormChance -= 5f;
                    cloudyChance += 10f;
                    break;
                case Season.Winter:
                    thunderStormChance = 0f;
                    rainyChance = 0f;
                    cloudyChance += 10f;
                    break;
            }
            
            for (int i = 0; i < 28; i++)
            {
                List<Weather> list = new List<Weather>();
                
                //float dailyTempurature = YearlyTempuratureChange.Evaluate(tempTime.GetProgressOfTheYearNormalized());
                //float upperend = dailyTempurature - (dailyTempurature * tempuratureVarianceFactor);
                //float lowerend = dailyTempurature - (dailyTempurature / tempuratureVarianceFactor);
                //float finalTemp = Random.Range(dailyTempurature + lowerend, dailyTempurature + upperend);
                for (int j = 0; j <(int)clearChance; j++)
                {
                    list.Add(Weather.Clear);
                }
                for(int j = 0;j < (int)cloudyChance; j++)
                {
                    list.Add(Weather.Cloudy);
                }
                for (int j = 0; j < (int)rainyChance; j++)
                {
                    list.Add(Weather.Raining);
                }
                for (int j = 0; j < (int)thunderStormChance; j++)
                {
                    list.Add(Weather.ThunderStorm);
                }
                weatherSaveData.weatherDictionary.Add(list[UnityEngine.Random.Range(0, list.Count)]);
            }
            ChangeWeather(time);

        }

        private void OnNewDay(DateTimeStamp time)
        {
            ChangeWeather(time);
        }


        private void ChangeWeather(DateTimeStamp time)
        {
            currentWeather = weatherSaveData.weatherDictionary[time.Day];
            OnWeatherChange?.Invoke(currentWeather);    

        }

        public void Bind(WeatherSaveData data)
        {
            weatherSaveData = data;
            weatherSaveData.Id = Id;
            currentWeather = weatherSaveData.currentWeather;
            if (weatherSaveData.weatherDictionary.Count <= 0)
            {
                OnNewSeason(TimeManager.Instance.CurrentGameTime);
            }
        }
        public void Save(ref WeatherSaveData data)
        {
            data.currentWeather = currentWeather;
        }
    }

  

}
