using GD.MinMaxSlider;
using UnityEngine;

namespace WorldTime {
    public enum TimeFormat {
        AM_PM,
        FullHours
    }

    public enum DayPeriod {
        Morning,
        Noon,
        Evening,
        Dusk,
        Night,
        Midnight
    }

    [CreateAssetMenu(fileName = "NewTimeSettings", menuName = "Assets/Time Settings/Time")]
    public class TimeSettings : ScriptableObject {
        /*[MinMaxSlider(0, 10000)]*/ public float timeMultiplier = 2000;
        /*[MinMaxSlider(0, 24)]*/ public int startDay = 0;
        /*[MinMaxSlider(0, 24)]*/ public float startHour = 5;
        /*[MinMaxSlider(0, 24)]*/ public float midnightHour = 0;
        /*[MinMaxSlider(0, 24)]*/ public float sunriseHour = 6;
        /*[MinMaxSlider(0, 24)]*/ public float noonHour = 12;
        /*[MinMaxSlider(0, 24)]*/ public float eveningHour = 16;
        /*[MinMaxSlider(0, 24)]*/ public float sunsetHour = 19;
        /*[MinMaxSlider(0, 24)]*/ public float nightHour = 21;
    }
}
