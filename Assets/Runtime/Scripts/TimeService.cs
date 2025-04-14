using System;
using ExtensionMethods;
using UnityEngine;

namespace WorldTime {
    public class TimeService {
        readonly TimeSettings timeSettings;
        DateTime currentTime;
        readonly TimeSpan midnightTime;
        readonly TimeSpan sunriseTime;
        readonly TimeSpan noonTime;
        readonly TimeSpan eveningTime;
        readonly TimeSpan sunsetTime;
        readonly TimeSpan nightTime;

        DayPeriod currentDayPeriod;

        public static event Action OnSunrise = delegate {};
        public static event Action OnNoon = delegate {};
        public static event Action OnEvening = delegate {};
        public static event Action OnSunset = delegate {};
        public static event Action OnNight = delegate {};
        public static event Action OnMidnight = delegate {};
        public static event Action OnHourChange = delegate {};
        public static event Action OnNewDay = delegate {};
        public static event Action OnDayPeriodChange = delegate {};

        // readonly Observable<bool> isDayTime;
        // readonly Observable<bool> isNightTime;
        // readonly Observable<bool> isPastNoon;
        // readonly Observable<bool> isPastMidnight;
        readonly Observable<int> currentHour;
        readonly Observable<int> currentDay;

        public TimeService(TimeSettings _settings) {
            timeSettings = _settings;

            currentTime = DateTime.Now.Date + TimeSpan.FromHours(timeSettings.startHour);
            midnightTime = TimeSpan.FromHours(timeSettings.midnightHour);
            sunriseTime = TimeSpan.FromHours(timeSettings.sunriseHour);
            noonTime = TimeSpan.FromHours(timeSettings.noonHour);
            eveningTime = TimeSpan.FromHours(timeSettings.eveningHour);
            sunsetTime = TimeSpan.FromHours(timeSettings.sunsetHour);
            nightTime = TimeSpan.FromHours(timeSettings.nightHour);

            // isDayTime = new Observable<bool>(IsDayTime());
            // isNightTime = new Observable<bool>(IsNightTime());
            // isPastNoon = new Observable<bool>(IsPastNoon());
            // isPastMidnight = new Observable<bool>(IsPastMidnight());
            currentHour = new Observable<int>(currentTime.Hour);
            currentDay = new Observable<int>(CurrentDay);
            // currentDay = new Observable<int>(currentTime.Day);

            // isDayTime.ValueChanged += day => (day ? OnSunrise : OnSunset)?.Invoke();
            currentHour.ValueChanged += _ => OnHourChange?.Invoke();
            currentDay.ValueChanged += _ => OnNewDay?.Invoke();
            CurrentDay = _settings.startDay;
        }

        public void SwapTimeSetting(TimeSettings settings) {

        }

        public void UpdateTime(float deltaTime) {
            currentTime = currentTime.AddSeconds(deltaTime);
            // isDayTime.Value = IsDayTime();
            // isNightTime.Value = IsNightTime();
            // isPastNoon.Value = IsPastNoon();
            // isPastMidnight.Value = IsPastMidnight();
            currentHour.Value = currentTime.Hour;
            currentDay.Value = CurrentDay;

            CalculateDayPeriod();
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

        public DateTime CurrentTime => currentTime;
        public int CurrentDay;
        // public DateTime CurrentDay => currentDay;
        public DayPeriod DayPeriod => currentDayPeriod;
        public bool IsDay => IsDayTime();
        public bool IsNight => IsNightTime();
        public bool IsNoon => IsPastNoon();
        public bool IsMidnight => IsPastMidnight();
        public float PercentOfDay => CalculatePercentOfDay(currentTime.TimeOfDay);

        bool IsDayTime() => currentTime.TimeOfDay >= sunriseTime && currentTime.TimeOfDay < sunsetTime;
        bool IsNightTime() => currentTime.TimeOfDay >= sunsetTime && currentTime.TimeOfDay < sunriseTime;
        bool IsPastNoon() => currentTime.TimeOfDay >= noonTime && currentTime.TimeOfDay < midnightTime;
        bool IsPastMidnight() => currentTime.TimeOfDay >= midnightTime && currentTime.TimeOfDay < noonTime;

        void CalculateDayPeriod() {
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

        void NewDay() => CurrentDay += 1;

        float CalculatePercentOfDay(TimeSpan timeSpan) {
            return (float)timeSpan.TotalSeconds % WorldTimeConstants.SecondsInDay / WorldTimeConstants.SecondsInDay;
        }

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
}
