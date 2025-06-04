using System;
using UnityEngine;
using UnityUtilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class TreeObject : WorldObject, IInteractable, IDestroyable, IItemSpawner {
    [field: SerializeField] public Collider2D InteractionCollider { get; set ; }
    [field: SerializeField] public bool IsInteractable { get; set; } = true;
    [field: SerializeField] public bool HasInteractabilityCooldown { get; set; } = true;
    [field: SerializeField] public CountdownTimer InteractabilityTimer { get; set; }
    [field: SerializeField, Range(0f, 10f)] public float InteractabilityCooldownSeconds { get; set; } = 0.5f;
    
    [property: SerializeField] public int CurrentHealth {
        get => GetObjectData<TreeData>().TreeCurrentHealth;
        set => GetObjectData<TreeData>().TreeCurrentHealth = value;
    }
    [field: SerializeField] public int MaxHealth { get; set; } = 5;

    [field: SerializeField] public List<LootTableSO> LootTablesList { get; set; }
    [field: SerializeField] public float SpawnRadius { get; set; } = 1f;
    [field: SerializeField] public bool DelaySpawn { get; set ; }
    [field: SerializeField] public float SpawnTime { get; set; } = 0.5f;

    [ExecuteInEditMode]
    void OnValidate() {
        if (worldObjectData.IsNull()) SetObjectData(new TreeData());
        worldObjectData.Name = "Tree";
        UpdateHealth(MaxHealth);
    }

    void OnEnable() {
        InteractabilityTimer.OnTimerStart += () => IsInteractable = false;
        InteractabilityTimer.OnTimerStop += () => {
            IsInteractable = true;
        };
    }

    void OnDisable() {
        InteractabilityTimer.OnTimerStart -= () => IsInteractable = false;
        InteractabilityTimer.OnTimerStop -= () => { IsInteractable = true; };
        InteractionSystem.Instance.InteractableTimersManager?.DeregisterTimer(InteractabilityTimer);
    }

    public override void Awake() {
        SetObjectData(new TreeData());
        worldObjectData.Name = "Tree";
        UpdateHealth(MaxHealth);
        InteractabilityTimer = new CountdownTimer(InteractabilityCooldownSeconds, ref InteractionSystem.Instance.InteractableTimersManager);
    }

    public override void Init() {
        UpdateHealth(GetObjectData<TreeData>().TreeCurrentHealth);
        InteractabilityTimer.Restart();
    }

    public void UpdateHealth(int health) {
        CurrentHealth = health;
    }

    public void OnInteract() {
        if (!IsInteractable) return;

        InteractionSystem.Instance.InteractableTimersManager.RegisterTimer(InteractabilityTimer);
        InteractabilityTimer.Restart();

        ChopTree();
    }

    public void ChopTree() {
        UpdateHealth(CurrentHealth - 1);

        if (CurrentHealth <= 0) {
            OnDestroyed();
        }
        else {

        }
    }

    public async void OnDestroyed() {
        SpriteRenderer.enabled = false;
        InteractionCollider.enabled = false;
        await CreateResources();
        WorldGenerator.Instance.RemoveObject(worldObjectData.ObjectPlacementData.OriginalPosition);
    }

    public async Task CreateResources() {
        if (LootTablesList.IsNullOrEmpty()) {
            Debug.Log("No Loot Tables to draw from!");
            await Task.Yield();
            return;
        }

        List<(LootDrop,int)> allLootDropsList = new();
        int totalDrops = 0;

        lock (LootTablesList) {
            foreach (LootTableSO lootTable in LootTablesList) {
                if (!lootTable.lootTableEnabled) continue;

                List<(LootDrop,int)> lootTableDrops = lootTable.GetDropItems();

                if (lootTableDrops.IsNullOrEmpty()) {
                    continue;
                }

                for (int i = 0; i < lootTableDrops.Count; i++) {
                    (LootDrop,int) lootDrop = lootTableDrops.GetElement(i);
                    int dropAmount = lootDrop.Item2;
                    allLootDropsList.Add(lootDrop);
                    totalDrops += dropAmount;
                }
            }
        }

        if (totalDrops == 0) {
            Debug.Log("No Loot dropped!");
            await Task.Yield();
            return;
        }
        else {
            Vector3 spawnLocation = transform.position;

            float spawnDelay = SpawnTime / totalDrops;

            if (!DelaySpawn) AudioManager.Instance.PlaySFX("Pop", false);

            for (int i = 0; i < allLootDropsList.Count; i++) {
                LootDrop lootDrop = allLootDropsList[i].Item1;
                int dropAmount = allLootDropsList[i].Item2;

                for (int j = 0; j < dropAmount; j++) {
                    Instantiate(lootDrop.itemPrefab, (UnityEngine.Random.insideUnitCircle * SpawnRadius) + spawnLocation.ToVector2(), Quaternion.identity, null);
                    // Item item = itemToSpawn.GetComponentInHierarchy<Item>();

                    if (DelaySpawn) {
                        AudioManager.Instance.PlaySFX("Pop", false);
                        await WaitFor.Delay(spawnDelay);
                    }
                    else {
                        await WaitFor.Delay(0.001f);
                    }
                }
            }
        }

        await Task.Yield();
    }
}