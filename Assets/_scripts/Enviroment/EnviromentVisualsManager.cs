using GameSystems.WaetherSystem;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EnviromentVisualsManager : MonoBehaviour
{
    public Light2D light2D;
    
    public Gradient grd;
    [SerializeField] private ParticleSystem rainParticalSystem;

    [SerializeField] private Weather currentWeather = Weather.Clear;
    [SerializeField] private Color rainingColor;
    [SerializeField] private Color cloudyColor;
    [SerializeField] private Color thunderstormColor;
    
    private void Start()
    {
        WeatherManager.OnWeatherChange += ChangeWeather;
        ChangeWeather(WeatherManager.CurrentWeather);
    }
    private void OnDisable()
    {
        WeatherManager.OnWeatherChange -= ChangeWeather;
    }

    private void ChangeWeather(Weather weather)
    {
        currentWeather = weather;
        
    }

    private void Update()
    {
        Color currentColor = Color.white;
        if(currentWeather == Weather.Clear)
        {
            rainParticalSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        else if(currentWeather == Weather.Cloudy)
        {
            currentColor = cloudyColor ;
            rainParticalSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        else if(currentWeather == Weather.Raining )
        {
            currentColor = rainingColor ;
            rainParticalSystem.Play(true);
        }
        else if(currentWeather == Weather.ThunderStorm)
        {
            currentColor = thunderstormColor ;
            rainParticalSystem.Play(true);
        }
        currentColor = Color.Lerp(currentColor, grd.Evaluate(TimeManager.Instance.TimeOfDayNormalized()), 0.5f);
        light2D.color = currentColor;

    }


}
