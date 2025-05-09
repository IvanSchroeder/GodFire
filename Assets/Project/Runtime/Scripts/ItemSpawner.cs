using System.Collections.Generic;
using UnityUtilities;
using UnityEngine;
using WorldSimulation;

public class ItemSpawner : MonoBehaviour {
    [SerializeField] List<GameObject> ItemsToSpawnList;
    [SerializeField] List<Transform> SpawnLocationsList;
    [SerializeField] AnimationCurve SpawnAmountMultCurve;
    [SerializeField] int baseItemsToSpawn;
    public float spawnRadius = 1f;

    int itemSpawned;

    public float spawnSeconds;
    CountdownTimer spawnTimer;
    GameObject itemToSpawn;
    Transform spawnLocation;
    int spawnAmount;

    void OnEnable() {
        // TimeManager.OnSunrise += TickItemSpawn;
        // TimeManager.OnEvening += TickItemSpawn;
        TimeManager.OnGameStart += TickItemSpawn;
        TimeManager.OnDayPeriodChange += TickItemSpawn;
        spawnTimer.OnTimerStop += SpawnItem;
    }

    void OnDisable() {
        // TimeManager.OnSunrise -= TickItemSpawn;
        // TimeManager.OnEvening -= TickItemSpawn;
        TimeManager.OnGameStart -= TickItemSpawn;
        TimeManager.OnDayPeriodChange -= TickItemSpawn;
        spawnTimer.OnTimerStop -= SpawnItem;
    }

    void Awake() {
        spawnTimer = new CountdownTimer(spawnSeconds);
    }

    void Update() {
        spawnTimer.Tick(Time.deltaTime);
    }

    void TickItemSpawn() {
        itemSpawned = 0;
        spawnAmount = (SpawnAmountMultCurve.Evaluate(Random.Range(0f, 1f)) * baseItemsToSpawn).ToIntRound();
        SpawnItem();
    }

    void SpawnItem() {
        if (itemSpawned < spawnAmount) {
            itemToSpawn = ItemsToSpawnList.GetRandomElement();
            spawnLocation = SpawnLocationsList.GetRandomElement();
            Instantiate(itemToSpawn, (Random.insideUnitCircle * spawnRadius) + spawnLocation.position.ToVector2(), Quaternion.identity, null);
            AudioManager.Instance.PlaySFX("Pop", false);
            itemSpawned++;
            spawnTimer.Restart();
        }
    }
}
