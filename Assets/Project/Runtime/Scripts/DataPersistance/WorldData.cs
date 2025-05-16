using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityUtilities;

[Serializable]
public class WorldData {
    [SerializeField] string _worldName = "New World";
    [SerializeField] int _worldSeed = 0;
    [SerializeField] bool _firstGeneration = true;
    [SerializeField] ChunkData _chunkData = new();
    [SerializeField] GridData _gridData = new();
    [SerializeField] DateTime _creationTime = DateTime.Now;
    [SerializeField] DateTime _lastSavedTime = DateTime.Now;

    [JsonIgnore] public string WorldName { get => _worldName; set => _worldName = value; }
    [JsonIgnore] public int WorldSeed { get => _worldSeed; set => _worldSeed = value; }
    [JsonIgnore] public bool FirstGeneration { get => _firstGeneration; set => _firstGeneration = value; }
    [JsonIgnore] public ChunkData ChunkData { get => _chunkData; set => _chunkData = value; }
    [JsonIgnore] public GridData GridData { get => _gridData; set => _gridData = value; }
    [JsonIgnore] public DateTime CreationTime { get => _creationTime; set => _creationTime = value; }
    [JsonIgnore] public DateTime LastSavedTime { get => _lastSavedTime; set => _lastSavedTime = value; }

    public WorldData() {
        _worldName = "New World";
        _worldSeed = 0;
        _chunkData = new();
        _gridData = new();
        _creationTime = DateTime.Now;
        _lastSavedTime = DateTime.Now;
    }

    public WorldData(bool randomizeSeed) {
        _worldName = "New World";
        int worldSeed = (randomizeSeed ? GenerateSeed() : 0).Clamp(int.MinValue, int.MaxValue);
        _worldSeed = worldSeed;
        _chunkData = new();
        _gridData = new();
        _creationTime = DateTime.Now;
        _lastSavedTime = DateTime.Now;
    }

    public WorldData(bool isNew, DateTime dateTime, bool randomizeSeed = true, int customSeed = 0) {
        if (isNew) {
            int worldSeed = (randomizeSeed ? GenerateSeed() : customSeed).Clamp(int.MinValue, int.MaxValue);
            _firstGeneration = true;
            _worldSeed = worldSeed;
            _creationTime = dateTime;
        }

        _lastSavedTime = dateTime;
    }

    public int GenerateSeed() {
        return UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }
}