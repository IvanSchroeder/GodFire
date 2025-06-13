using System;

namespace WorldSimulation {
    /// <summary>
    /// Extensions for TimeSpan and DateTime
    /// </summary>
    public static class TimeSpanExtensions {
        public static float GetPercentOfDay(this TimeSpan timeSpan) {
            return (float)timeSpan.TotalSeconds % WorldTimeConstants.SecondsInDay / WorldTimeConstants.SecondsInDay;
        }

        public static float GetPercentOfTime(this TimeSpan startTime, TimeSpan endTime) {
            return (float)(endTime - startTime).TotalSeconds % WorldTimeConstants.SecondsInDay / WorldTimeConstants.SecondsInDay;
        }

        public static TimeSpan GetTimeOfDay(float percentage) {
            var startTime = new TimeSpan(0, 0, 0);
            var endTime = new TimeSpan(24, 0, 0);
            var percentageInTicks = (long)((endTime - startTime).Ticks * percentage);
            return startTime.Add(TimeSpan.FromTicks(percentageInTicks));
        }

        public static TimeSpan GetTimeOfDay(double percentage) {
            var startTime = new TimeSpan(0, 0, 0);
            var endTime = new TimeSpan(24, 0, 0);
            var percentageInTicks = (long)((endTime - startTime).Ticks * percentage);
            return startTime.Add(TimeSpan.FromTicks(percentageInTicks));
        }

        public static TimeSpan GetTimeOfDay(float percentage, TimeSpan startTime, TimeSpan endTime) {
            var percentageInTicks = (long)((endTime - startTime).Ticks * percentage);
            return startTime.Add(TimeSpan.FromTicks(percentageInTicks));
        }

        public static TimeSpan GetTimeOfDay(double percentage, TimeSpan startTime, TimeSpan endTime) {
            var percentageInTicks = (long)((endTime - startTime).Ticks * percentage);
            return startTime.Add(TimeSpan.FromTicks(percentageInTicks));
        }

        public static TimeSpan GetTimeOfDay(float percentage, DateTime startDate, DateTime endDate) {
            var percentageInTicks = (long)((endDate - startDate).Ticks * percentage);
            return startDate.TimeOfDay.Add(TimeSpan.FromTicks(percentageInTicks));
        }

        public static TimeSpan GetTimeOfDay(double percentage, DateTime startDate, DateTime endDate) {
            var percentageInTicks = (long)((endDate - startDate).Ticks * percentage);
            return startDate.TimeOfDay.Add(TimeSpan.FromTicks(percentageInTicks));
        }
    }
}
