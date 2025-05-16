using System;
using UnityEngine;
using WorldSimulation;
using Newtonsoft.Json;

[Serializable]
public class TimeData {
    [SerializeField] DateTime _worldTime = DateTime.MinValue;
    [SerializeField] int _worldDay = 0;

    [JsonIgnore] public DateTime WorldTime { get => _worldTime; set => _worldTime = value; }
    [JsonIgnore] public int WorldDay { get => _worldDay; set => _worldDay = value; }

    public TimeData() {
        _worldTime = DateTime.MinValue;
        _worldDay = 0;
    }

    public TimeData(DateTime worldTime, int worldDay) {
        _worldTime = worldTime;
        _worldDay = worldDay;
    }
}