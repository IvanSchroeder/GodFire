using System;
using UnityEngine;
using WorldSimulation;

public class WeatherData {
    [SerializeField] WeatherType _weatherType;
    [SerializeField] bool _isRaining;
    [SerializeField] float _rainChance;
    [SerializeField] float _rainTime;
    [SerializeField] float _maxRainTime;

    public WeatherType WeatherType { get => _weatherType; set => _weatherType = value; }
    public bool IsRaining { get => _isRaining; set => _isRaining = value; }
    public float RainChance { get => _rainChance; set => _rainChance = value; }
    public float RainTime { get => _rainTime; set => _rainTime = value; }
    public float MaxRainTime { get => _maxRainTime; set => _maxRainTime = value; }

    public WeatherData() {}

    public WeatherData(WeatherType type, bool isRaining, float rainChance, float rainTime, float maxRainTime) {
        _weatherType = type;
        _isRaining = isRaining;
        _rainChance = rainChance;
        _rainTime = rainTime;
        _maxRainTime = maxRainTime;
    }
}