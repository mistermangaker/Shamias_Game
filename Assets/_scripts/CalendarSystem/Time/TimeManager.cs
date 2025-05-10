using Callendar;
using GameSystems.SaveLoad;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class TimeSaveData : ISaveable
{
    [field: SerializeField]
    public SerializableGuid Id { get; set; }
    public DateTimeStamp timestamp;
}
public class TimeManager : MonoBehaviour, IBind<TimeSaveData>
{
    public static TimeManager Instance { get; private set; }

    [SerializeField] private AnimationCurve lightintensitiy;

    [Header("Time Management")]
    [field: SerializeField] private DateTimeStamp starttime;
    [SerializeField] private float secondsBetweenTicks = 6f;
    private float currentTimeBetweenTicks = 0f;

    [SerializeField] private int MinutesToAdvanceTicksBy = 10;
    
    private DateTimeStamp _currentGameTime;
    public DateTimeStamp CurrentGameTime => _currentGameTime;
    public DateTimeStamp TodaysDate => _currentGameTime.GetCurrentDate();

    [Header("Save Information")]
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
    [SerializeField] public TimeSaveData timeSaveData = new TimeSaveData();


   



    public static UnityAction<DateTimeStamp> OnTimeChanged;
    public static UnityAction<DateTimeStamp> OnNewDay;
    public static UnityAction<DateTimeStamp> OnNewWeek;
    public static UnityAction<DateTimeStamp> OnNewSeason;
    public static UnityAction<DateTimeStamp> OnNewYear;
    [ContextMenu("Set CurrentTime")]
    private void SetTime()
    {
        _currentGameTime = new DateTimeStamp(starttime.Year, (int)starttime.Season, starttime.Day, starttime.Hour, starttime.Minute);
        timeSaveData.timestamp = CurrentGameTime;
    }
    
    private void Awake()
    {
        Instance = this;
        
        SetTime();
    }

    public float TimeOfDayNormalized()
    {
        
        return ((secondsBetweenTicks * (currentTimeBetweenTicks / secondsBetweenTicks)) + (secondsBetweenTicks * _currentGameTime.TotalMinutes)) / (secondsBetweenTicks * 1440);
    }

    public float GetLightInesityForTimeOfDay()
    {
        return lightintensitiy.Evaluate(TimeOfDayNormalized());
    }

    private void FixedUpdate()
    {
        currentTimeBetweenTicks -= Time.deltaTime;
        if(currentTimeBetweenTicks < 0)
        {
            currentTimeBetweenTicks = secondsBetweenTicks;
            Tick();
        }
    }

    private void Tick()
    {
        Days dayOfTheWeek = _currentGameTime.Days;
        Season CurrentSeason = _currentGameTime.Season;
        int year = _currentGameTime.Year;
        int day = _currentGameTime.Day;
        _currentGameTime.AdvanceMinute(MinutesToAdvanceTicksBy);
        int dayTwo = _currentGameTime.Day;
        Season CurrentSeasonTwo = _currentGameTime.Season;
        int yearTwo = _currentGameTime.Year;

        OnTimeChanged?.Invoke(CurrentGameTime);
        if(day != dayTwo)
        {
            OnNewDay?.Invoke(CurrentGameTime);
            if(dayOfTheWeek == Days.Sunday)
            {
                OnNewWeek?.Invoke(CurrentGameTime);
            }
            if (CurrentSeason != CurrentSeasonTwo)
            {
                Debug.Log("new season");
                OnNewSeason?.Invoke(CurrentGameTime);

                if (year != yearTwo)
                {
                    OnNewYear?.Invoke(CurrentGameTime);
                }
            }
        }

        timeSaveData.timestamp = CurrentGameTime;
    }

    public void Bind(TimeSaveData data)
    {
        //Debug.Log("binding");
        this.timeSaveData = data;
        timeSaveData.Id =Id;
        if(!(data.timestamp.Equals(DateTimeStamp.Empty)))
        {
            _currentGameTime = timeSaveData.timestamp;
        }
    }

    // save data is already bound to the current save game data
    //public void Save(ref TimeSaveData data)
    //{
    //}
}
