using System;
using UnityEngine;
using WorldSimulation;
using Newtonsoft.Json;

[Serializable]
public class WeatherData {
    [SerializeField] WeatherType _weatherType = WeatherType.Clear;
    [SerializeField] bool _isRaining = false;
    [SerializeField] float _rainChance = 0.1f;
    [SerializeField] float _rainTime = 0;
    [SerializeField] float _maxRainTime = 0;

    [JsonIgnore] public WeatherType WeatherType { get => _weatherType; set => _weatherType = value; }
    [JsonIgnore] public bool IsRaining { get => _isRaining; set => _isRaining = value; }
    [JsonIgnore] public float RainChance { get => _rainChance; set => _rainChance = value; }
    [JsonIgnore] public float RainTime { get => _rainTime; set => _rainTime = value; }
    [JsonIgnore] public float MaxRainTime { get => _maxRainTime; set => _maxRainTime = value; }

    public WeatherData() {
        _weatherType = WeatherType.Clear;
        _isRaining = false;
        _rainChance = 0.1f;
        _rainTime = 0;
        _maxRainTime = 0;
    }

    public WeatherData(WeatherType type, bool isRaining, float rainChance, float rainTime, float maxRainTime) {
        _weatherType = type;
        _isRaining = isRaining;
        _rainChance = rainChance;
        _rainTime = rainTime;
        _maxRainTime = maxRainTime;
    }
}