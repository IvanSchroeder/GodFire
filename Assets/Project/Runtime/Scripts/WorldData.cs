using System;
using UnityEngine;

[Serializable]
public class WorldData {
    [SerializeField] string _worldName;
    [SerializeField] int _worldSeed;
    [SerializeField] ChunkData _chunkData;
    [SerializeField] bool _generatedHeightMap = false;
    [SerializeField] HeightMap _heightMap;
    [SerializeField] float[,] _heightValues;
    [SerializeField] DateTime _creationTime = DateTime.Now;
    [SerializeField] DateTime _lastSavedTime = DateTime.Now;

    public string WorldName {
        get {
            return _worldName;
        }
        set {
            _worldName = value;
        }
    }
 
    public int WorldSeed {
        get {
            return _worldSeed;
        }
        set {
            _worldSeed = value;
        }
    }

    public ChunkData ChunkData {
        get {
            return _chunkData;
        }
        set {
            _chunkData = value;
        }
    }

    public bool GeneratedHeightMap {
        get {
            return _generatedHeightMap;
        }
        set {
            _generatedHeightMap = value;
        }
    }

    public HeightMap HeightMap {
        get {
            return _heightMap;
        }
        set {
            _heightMap = value;
        }
    }

    public float[,] HeightValues {
        get {
            return _heightValues;
        }
        set {
            _heightValues = value;
        }
    }

    public DateTime CreationTime {
        get {
            return _creationTime;
        }
        set {
            _creationTime = value;
        }
    }

    public DateTime LastSavedTime {
        get {
            return _lastSavedTime;
        }
        set {
            _lastSavedTime = value;
        }
    }

    public WorldData() {
        _chunkData = new ChunkData();
        _creationTime = DateTime.Now;
        _lastSavedTime = DateTime.Now;
        _generatedHeightMap = false;
    }

    public WorldData(bool isNew, DateTime dateTime) {
        _chunkData = new ChunkData();

        if (isNew) {
            _worldSeed = GenerateSeed();
            _creationTime = DateTime.Now;
            _heightMap = new HeightMap();
            _generatedHeightMap = false;
        }

        SetSavedTime(dateTime);
    }

    public void SetSavedTime(DateTime time) => _lastSavedTime = time;

    public int GenerateSeed() {
        return UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }
}