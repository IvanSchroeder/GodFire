using UnityEngine;
using System;
using Newtonsoft.Json;

namespace SaveSystem {
    [Serializable]
    public class GameData {
        [SerializeField] WorldData _worldData = null;
        [SerializeField] TimeData _timeData = null;
        [SerializeField] WeatherData _weatherData = null;

        [JsonIgnore] public WorldData WorldData { get => _worldData; set => _worldData = value; }
        [JsonIgnore] public TimeData TimeData { get => _timeData; set => _timeData = value; }
        [JsonIgnore] public WeatherData WeatherData { get => _weatherData; set => _weatherData = value; }

        public GameData() {
            WorldData = null;
            TimeData = null;
            WeatherData = null;
        }
    }
}