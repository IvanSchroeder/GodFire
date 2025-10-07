using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityUtilities;

[Serializable]
public class WorldData {
    [SerializeField] string _worldName = "New World";
    [SerializeField] int _worldSeed = 0;
    [SerializeField] bool _firstGeneration = true;
    [SerializeField] GridData _gridData = new();
    [SerializeField] ChunkData _chunkData = new();
    [SerializeField] List<ItemData> _itemDataList = new();
    [SerializeField] DateTime _creationTime = DateTime.Now;
    [SerializeField] DateTime _lastSavedTime = DateTime.Now;

    [JsonIgnore] public string WorldName { get => _worldName; set => _worldName = value; }
    [JsonIgnore] public int WorldSeed { get => _worldSeed; set => _worldSeed = value; }
    [JsonIgnore] public bool FirstGeneration { get => _firstGeneration; set => _firstGeneration = value; }
    [JsonIgnore] public ChunkData ChunkData { get => _chunkData; set => _chunkData = value; }
    [JsonIgnore] public GridData GridData { get => _gridData; set => _gridData = value; }
    [JsonIgnore] public List<ItemData> ItemDataList { get => _itemDataList; set => _itemDataList = value; }
    [JsonIgnore] public DateTime CreationTime { get => _creationTime; set => _creationTime = value; }
    [JsonIgnore] public DateTime LastSavedTime { get => _lastSavedTime; set => _lastSavedTime = value; }

    public WorldData() {}

    public WorldData(bool isNew, DateTime dateTime, string worldName = "", bool randomizeSeed = true, int customSeed = 0) {
        if (isNew) {
            _worldName = worldName;
            _worldSeed = (randomizeSeed ? GenerateSeed() : customSeed).Clamp(int.MinValue, int.MaxValue);
            _firstGeneration = true;
            _creationTime = dateTime;

            Debug.Log($"Created {_worldName} seeded {_worldSeed}");
        }

        _lastSavedTime = dateTime;
    }

    public int GenerateSeed() {
        return UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }

    // public void RegisterItemData(ItemData itemData) {
    //     if (!ItemDataList.Contains(itemData)) ItemDataList.Add(itemData);
    // }

    // public void DeregisterItemData(ItemData itemData) {
    //     if (ItemDataList.Contains(itemData)) ItemDataList.Remove(itemData);
    // }
}