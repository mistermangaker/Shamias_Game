using System;
using UnityEngine;

namespace Callendar
{
    
    public static class DateTimeUtility
    {
        
        //public static DateTimeStamp GetDateInFuture(DateTimeStamp a, DateTimeStamp b)
        //{

        //}

    }
    [System.Serializable]
    public struct DateTimeStamp
    {
        [field: SerializeField]
        public int Day { get; private set; }
        [field: SerializeField]
        public Days Days { get; private set; }
        [field: SerializeField]
        public int Year { get; private set; }
        [field: SerializeField]
        public Season Season { get; private set; }
        [field: SerializeField]
        public int Hour { get; private set; }
        [field: SerializeField]
        public int Minute { get; private set; }
        [field: SerializeField]
        public int TotalMinutes { get; private set; }
        [field: SerializeField]
        public int DayOfTheYear { get; private set; }


        public float GetProgressOfTheYearNormalized()
        {
            return (float)DayOfTheYear / 112f;
        }

        public DateTimeStamp GetCurrentDate()
        {
            return new DateTimeStamp(Year, (int)Season, Day, 0, 0);
        }
        public DateTimeStamp Never
        {
            get
            {
                return new DateTimeStamp(9999,0 ,0, 0, 0);
            }
        }

        public DateTimeStamp(int year, int season, int day, int hour, int minute)
        {
            Day = day;
            Days = (Days)(day % 7);
            if (Day == 0) Days = (Days)7;
            Year = year;
            
            Season = (Season)season;
            Hour = hour;
            Minute = minute;
            TotalMinutes = (Hour*60 + Minute);
            DayOfTheYear = (season * 28) + day;
        }

        public static DateTimeStamp Empty
        {
            get
            {
                return new DateTimeStamp(0, 0, 0, 0, 0);
            }
        }

        public void AdvanceMinute(int MinutesToAdvanceBy)
        {
            var minutes = Minute + MinutesToAdvanceBy;
            TotalMinutes += MinutesToAdvanceBy;
            while (minutes >= 60)
            {
                minutes -= 60;
                AdvanceHour(1);
            }
            Minute = minutes;
        }

      
        public void AdvanceHour(int HoursToAdvanceBy)
        {
            var hours = Hour + HoursToAdvanceBy;
            if(hours >= 24)
            {
                Debug.Log("dayTick");
                hours -= 24;
                AdvanceDay(1);
            }
            Hour = hours;
        }

        public void AdvanceDay(int DaysToAdvanceBy)
        {
            var days = Day + DaysToAdvanceBy;
            while(days > 28)
            {
                days -= 28;
                AdvanceSeason();
            }
            Day = days;
            Days = (Days)(Day % 7);
            if (Day == 0) Days = (Days)7;
           
            TotalMinutes = (Hour * 60 + Minute);
        }

        private void AdvanceSeason()
        {
            if (Season == Season.Winter)
            {
                Season = Season.Spring;
                AdvanceYear(1);
            }
            else
            {
                Season++;
            }
        }

        public void JumpToNextDayInHoursAndMinutes(int hours,int minutes = 0)
        {
            Day++;
            Hour = hours;
            Minute = minutes;
            TotalMinutes = ((Hour * 60) + Minute);
        }

        public int CalulateTotalMinutes() => ((((Year * 112) + DayOfTheYear) * 24) * 60) + TotalMinutes;

        public static float TimeBetweenDateTimeStampsNormalized(DateTimeStamp a, DateTimeStamp b)
        {
            return (float)a.CalulateTotalMinutes() / (float)b.CalulateTotalMinutes();
        }

        public static bool operator ==(DateTimeStamp left , DateTimeStamp right) => left.Equals(right);
        public static bool operator !=(DateTimeStamp left , DateTimeStamp right) => !(left.Equals(right));

        public static bool operator <=(DateTimeStamp left, DateTimeStamp right) => !(left.GreaterThan(right));
        public static bool operator >=(DateTimeStamp left, DateTimeStamp right) => (left.GreaterThan(right));

        public bool GreaterThan(DateTimeStamp other)
        {
            if(other.Year>this.Year) return false;
            if(other.DayOfTheYear >  this.DayOfTheYear) return false;
            if(other.TotalMinutes > this.TotalMinutes) return false;
            return true;
        }

        private void AdvanceYear(int YearsToAdvanceBy)
        {
            Year += YearsToAdvanceBy;
            Day = 1;
        }
        public override bool Equals(object obj)
        {
            return obj is DateTimeStamp datetime && this.Equals(datetime);
        }

        public bool Equals(DateTimeStamp other)
        {
            return this.Year == other.Year && this.Season == other.Season && this.Day == other.Day && this.Hour == other.Hour && this.Minute == other.Minute;
        }

        public enum ComparisionTo
        {
            ToYear,
            ToSeason, 
            ToDay,
            ToHour,
            ToMinute,
        }

        public bool Equals(DateTimeStamp other , ComparisionTo comparetodate)
        {
            switch (comparetodate)
            {
                case ComparisionTo.ToYear:
                    return Equalsyear(other);
                case ComparisionTo.ToSeason:
                    return Equalsyear(other) && EqualsSeason(other);
                case ComparisionTo.ToDay:
                    return Equalsyear(other) && EqualsSeason(other) && EqualsDay(other);
                case ComparisionTo.ToHour:
                    return Equalsyear(other) && EqualsSeason(other) && EqualsDay(other) && EqualsHour(other);
                default:
                case ComparisionTo.ToMinute:
                    return Equalsyear(other) && EqualsSeason(other) && EqualsDay(other) && EqualsHour(other) && EqualsMinute(other);
            }
        }

        private bool Equalsyear(DateTimeStamp other)
        {
            if (other.Year != this.Year) return false;
            return true;
        }

        private bool EqualsSeason(DateTimeStamp other)
        {
            if(other.Season != this.Season) return false;
            return true;
        }
        private bool EqualsDay(DateTimeStamp other)
        {
            if(other.Day != this.Day) return false;
            return true;
        }
        private bool EqualsHour(DateTimeStamp other)
        {
            if(other.Hour != this.Hour) return false;
            return true;
        }
        private bool EqualsMinute(DateTimeStamp other)
        {
            if(other.Minute != this.Minute) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Year, this.Season, this.Day, this.Hour, this.Minute);
        }
        public override string ToString()
        {
            return $"Date: {DateToString()} Season: {Season} Time: {TimeToString()}";
        }
        public string DateToSaveString()
        {
            return $"{Year}_{Season}_{Day}";
        }
        public string DateToString()
        {
            return $"{Days} The {DayOfMonthToString()} of {Season}";
        }

        public string DayOfMonthToString()
        {
            if( Day == 1)
            {
                return $"{Day}st";
            }
            if( Day == 2)
            {
                return $"{Day}nd";
            }
            return $"{Day}th";
        }
        public string TimeToString()
        {
            int houradjustment = 1;
            if (Hour == 0)
            {
                houradjustment = 12;
            }
            else if (Hour == 24)
            {
                houradjustment = 12;
            }
            else if (Hour > 12)
            {
                houradjustment = Hour - 12;
            }
            else
            {
                houradjustment = Hour;
            }
            string ampm = Hour == 0 || Hour < 12 ? "AM" : "PM";
            return $"{houradjustment.ToString("D2")}:{Minute.ToString("D2")} {ampm}";
        }

    }

    [System.Serializable]
    public enum Days
    {
        None = 0,
        Sunday =1,
        Monday = 2,
        Tuesday = 3,
        Wednesday = 4,
        Thursday = 5,
        Friday = 6,
        Saturday = 7,
    }

    [System.Serializable]
    public enum Season
    {
        Spring = 0,
        Summer = 1,
        Fall = 2,
        Winter = 3
    }

}
