using System;
using Newtonsoft.Json;
using UnityEngine;


namespace WorldSimulation {
    [Serializable]
    public enum WeatherType {
        Clear,
        Cloudy,
        MildRain,
        HeavyRain,
        StormRain
    }

    [CreateAssetMenu(fileName = "NewWeatherSettings", menuName = "Assets/Time Settings/Weather")]
    public class WeatherSettings : ScriptableObject {
        public string weatherTypeName = "Rain";
        public WeatherType weatherType = WeatherType.Clear;
        public bool startsRaining = true;
        public string rainStartLogMessage = $"It started raining!";
        public string rainStopLogMessage = $"Raining stopped...";
        public AnimationCurve rainTimeRangeCurve;
        public AnimationCurve rainSpeedCurve;
        public AnimationCurve rainAmountCurve;
        public float baseRainTimeHours = 18;
        public float baseRainSpeed = 2;
        public float baseRainAmount = 100;
        [JsonConverter(typeof(ColorHandler))] public Color weatherColor;
        public AudioClip audioClip;
        [Range(1f, 10f)] public float rainChanceCutoff = 2f;
        [Range(1f, 5f)] public float fuelConsuptionMultiplier = 2f;
    }
}
