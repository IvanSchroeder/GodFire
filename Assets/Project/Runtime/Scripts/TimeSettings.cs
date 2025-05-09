using System.ComponentModel;
using GD.MinMaxSlider;
using UnityEngine;

namespace WorldSimulation {
    public enum TimeFormat {
        AM_PM,
        FullHours,
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
        [Tooltip
        ("Time multiplier in seconds. Example: 1(tM) * Time.deltaTime = 24 Hours day (1 Real second = 1 In Game second), 60 * Time.deltaTime = 24 Minutes day (1 Real minute = 1 In Game Hour)"
        )]
        public float timeMultiplier = 2000;
    }
}
