using System;
using UnityUtilities;
using UnityEngine;

namespace WorldSimulation {
    public class TimeService {
        // readonly TimeSettings _timeSettings;
        // public TimeSettings TimeSettings { get => _timeSettings; }
        readonly TimeSpan midnightTime;
        readonly TimeSpan sunriseTime;
        readonly TimeSpan noonTime;
        readonly TimeSpan eveningTime;
        readonly TimeSpan sunsetTime;
        readonly TimeSpan nightTime;
        DateTime currentTime;
        DayPeriod currentDayPeriod;

        public DateTime CurrentTime => currentTime;
        public int CurrentDay;
        public DayPeriod DayPeriod => currentDayPeriod;
        public bool IsDayTime => currentTime.TimeOfDay >= sunriseTime && currentTime.TimeOfDay < sunsetTime;
        public bool IsNightTime => currentTime.TimeOfDay >= sunsetTime && currentTime.TimeOfDay < sunriseTime;
        public bool IsPastNoon => currentTime.TimeOfDay >= noonTime && currentTime.TimeOfDay < midnightTime;
        public bool IsPastMidnight => currentTime.TimeOfDay >= midnightTime && currentTime.TimeOfDay < noonTime;
        public float PercentOfDay => currentTime.TimeOfDay.GetPercentOfDay();
        public float PercentOfDayTime => currentTime.TimeOfDay.GetPercentOfDay();
        public float PercentOfDayNight => currentTime.TimeOfDay.GetPercentOfDay();

        readonly Observable<bool> isDayTime;
        readonly Observable<bool> isNightTime;
        readonly Observable<bool> isPastNoon;
        readonly Observable<bool> isPastMidnight;
        readonly Observable<int> currentHour;
        readonly Observable<int> currentDay;

        public static event Action OnSunrise = delegate {};
        public static event Action OnNoon = delegate {};
        public static event Action OnEvening = delegate {};
        public static event Action OnSunset = delegate {};
        public static event Action OnNight = delegate {};
        public static event Action OnMidnight = delegate {};
        public static event Action OnHourChange = delegate {};
        public static event Action OnNewDay = delegate {};
        public static event Action OnDayPeriodChange = delegate {};

        public TimeService() {}

        public TimeService(float startHour, int startDay, float midnightHour, float sunriseHour, float noonHour, float eveningHour, float sunsetHour, float nightHour) {
            currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);
            midnightTime = TimeSpan.FromHours(midnightHour);
            // midnightTime = new TimeSpan(days: 0, hours: 23, minutes: 59, seconds: 59, milliseconds: 999);
            sunriseTime = TimeSpan.FromHours(sunriseHour);
            noonTime = TimeSpan.FromHours(noonHour);
            eveningTime = TimeSpan.FromHours(eveningHour);
            sunsetTime = TimeSpan.FromHours(sunsetHour);
            nightTime = TimeSpan.FromHours(nightHour);

            currentHour = new Observable<int>(currentTime.Hour);
            currentDay = new Observable<int>(CurrentDay);
            // isDayTime = new Observable<bool>(IsDayTime());
            // isNightTime = new Observable<bool>(IsNightTime());
            // isPastNoon = new Observable<bool>(IsPastNoon());
            // isPastMidnight = new Observable<bool>(IsPastMidnight());
            // currentDay = new Observable<int>(currentTime.Day);

            currentHour.ValueChanged += _ => OnHourChange?.Invoke();
            currentDay.ValueChanged += _ => OnNewDay?.Invoke();
            // isDayTime.ValueChanged += day => (day ? OnSunrise : OnSunset)?.Invoke();
            CurrentDay = startDay;
        }

        public void SetCurrentTime(DateTime current) => currentTime = current;

        public void UpdateTime(float deltaTime) {
            SetCurrentTime(currentTime.AddSeconds(deltaTime));
            // isDayTime.Value = IsDayTime;
            // isNightTime.Value = IsNightTime;
            // isPastNoon.Value = IsPastNoon;
            // isPastMidnight.Value = IsPastMidnight;
            currentHour.Value = currentTime.Hour;

            CycleDayPeriod();
        }

        // public float CalculateSunAngle() {
        //     bool isDay = IsDayTime();
        //     float startDegree = isDay ? 0 : 180;
        //     TimeSpan start = isDay ? sunriseTime : sunsetTime;
        //     TimeSpan end = isDay ? sunsetTime : sunriseTime;

        //     TimeSpan totalTime = CalculateDifference(start, end);
        //     TimeSpan elapsedTime = CalculateDifference(start, currentTime.TimeOfDay);

        //     double percentage = elapsedTime.TotalMinutes / totalTime.TotalMinutes;
        //     return Mathf.Lerp(startDegree, startDegree + 180, (float) percentage);
        // }

        void CycleDayPeriod() {
            if (currentTime.TimeOfDay >= midnightTime && currentTime.TimeOfDay < sunriseTime) {
                SetDayPeriod(DayPeriod.Midnight);
            }
            else if (currentTime.TimeOfDay >= sunriseTime && currentTime.TimeOfDay < noonTime) {
                SetDayPeriod(DayPeriod.Morning);
            }
            else if (currentTime.TimeOfDay >= noonTime && currentTime.TimeOfDay < eveningTime) {
                SetDayPeriod(DayPeriod.Noon);
            }
            else if (currentTime.TimeOfDay >= eveningTime && currentTime.TimeOfDay < sunsetTime) {
                SetDayPeriod(DayPeriod.Evening);
            }
            else if (currentTime.TimeOfDay >= sunsetTime && currentTime.TimeOfDay < nightTime) {
                SetDayPeriod(DayPeriod.Dusk);
            }
            else if (currentTime.TimeOfDay >= nightTime && currentTime.TimeOfDay < TimeSpan.FromHours(WorldTimeConstants.HoursInDay)) {
                SetDayPeriod(DayPeriod.Night);
            }
        }

        void SetDayPeriod(DayPeriod _dayPeriod) {
            if (currentDayPeriod == _dayPeriod) return;

            currentDayPeriod = _dayPeriod;

            switch (currentDayPeriod) {
                case DayPeriod.Morning:
                    OnSunrise?.Invoke();
                    break;
                case DayPeriod.Noon:
                    OnNoon?.Invoke();
                    break;
                case DayPeriod.Evening:
                    OnEvening?.Invoke();
                    break;
                case DayPeriod.Dusk:
                    OnSunset?.Invoke();
                    break;
                case DayPeriod.Night:
                    OnNight?.Invoke();
                    break;
                case DayPeriod.Midnight:
                    OnMidnight?.Invoke();
                    NewDay();
                    break;
            }

            OnDayPeriodChange?.Invoke();
        }

        public void SetCurrentDay(int current) {
            CurrentDay = current;
            currentDay.Value = CurrentDay;
        }
        void NewDay() => SetCurrentDay(CurrentDay + 1);

        TimeSpan CalculateDifference(TimeSpan from, TimeSpan to) {
            TimeSpan difference = to - from;
            return difference.TotalHours < 0 ? difference + TimeSpan.FromHours(24) : difference;
        }
    }

    public static class WorldTimeConstants {
        public const int SecondsInMinute = 60;
        public const int SecondsInHour = 3600;
        public const int SecondsInDay = 86400;
        public const int MinutesInHour = 60;
        public const int MinutesInDay = 1440;
        public const int HoursInDay = 24;
        public const int DaysInWeek = 7;
        public const int MonthsInYear = 12;
    }
    
    // public class HourRange {
    //     public int minHour;
    //     public int maxHour;

    //     public bool IsInRange(int hour) {

    //         if (hour >= minHour && hour < maxHour) {
    //             return true;
    //         }

    //         return false;
    //     }
    // }
}
