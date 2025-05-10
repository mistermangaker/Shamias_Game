using TMPro;
using UnityEngine;
using Callendar;
using UnityEngine.UI;
using GameSystems.WaetherSystem;

public class INGameClockUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText, dayText, SeasonText, WeatherText;
    [SerializeField] private GameObject clockDialImage;
    private TimeManager timeManager;

    private void Start()
    {
        TimeManager.OnTimeChanged += UpdateTime;
        timeManager = TimeManager.Instance;


    }
    private void OnDisable()
    {
        TimeManager.OnTimeChanged -= UpdateTime;
    }

    private void UpdateTime(DateTimeStamp time)
    {
        float newRotation = Mathf.Lerp(0, 360, timeManager.TimeOfDayNormalized());
        clockDialImage.transform.rotation = new Quaternion(0f, 0f, newRotation, 0f);

        timeText.text = time.TimeToString();
        dayText.text = time.DateToString();
        SeasonText.text = "Year "+time.Year.ToString();
        WeatherText.text = "Weather: "+ WeatherManager.CurrentWeather.ToString();
    }
}
