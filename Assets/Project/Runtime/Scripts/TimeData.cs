using System;
using UnityEngine;
using WorldSimulation;

[Serializable]
public class TimeData {
    [SerializeField] DateTime _worldTime;
    [SerializeField] int _worldDay;

    public DateTime WorldTime { get => _worldTime; set => _worldTime = value; }
    public int WorldDay { get => _worldDay; set => _worldDay = value; }

    public TimeData() {}

    public TimeData(DateTime worldTime, int worldDay) {
        _worldTime = worldTime;
        _worldDay = worldDay;
    }
}