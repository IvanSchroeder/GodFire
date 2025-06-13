using System.Collections.Generic;
using AYellowpaper.SerializedCollections;

public interface IItemSpawner {
    // OnEvent Key, List of Tables Value
    SerializedDictionary<string, List<LootTableSO>> LootTablesDictionary { get; set; }
    float SpawnRadius { get; set; }
    bool DelaySpawn { get; set; }
    float SpawnTime { get; set; }
}