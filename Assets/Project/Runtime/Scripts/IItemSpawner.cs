using System.Collections.Generic;

public interface IItemSpawner {
    List<LootTableSO> LootTablesList { get; set; }
    float SpawnRadius { get; set; }
    bool DelaySpawn { get; set; }
    float SpawnTime { get; set; }
}