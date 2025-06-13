using System;
using UnityEngine;
using UnityUtilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using AYellowpaper.SerializedCollections;
using UnityEngine.EventSystems;

[Serializable]
public class TreeData : WorldObjectData {
    public float Health = 5;
}

public class TreeObject : WorldObject, IInteractable, IDestroyable, IItemSpawner {
    TreeData LocalTreeData { get => GetObjectData<TreeData>(); set => SetObjectData(value); }

#region Interactable
    [field: SerializeField] public InteractableTrigger InteractableTrigger { get; set ; }
#endregion

#region Destroyable
    [field: SerializeField] public float MaxHealth { get; set; } = 5;
#endregion

#region Item Spawner
    [field: SerializeField] public SerializedDictionary<string, List<LootTableSO>> LootTablesDictionary { get; set; } = new() {
        { ON_INTERACTED, null },
        { ON_DESTROYED, null },
    };
    [field: SerializeField] public float SpawnRadius { get; set; } = 1f;
    [field: SerializeField] public bool DelaySpawn { get; set ; }
    [field: SerializeField] public float SpawnTime { get; set; } = 0.5f;
#endregion

#if UNITY_EDITOR
    protected override void OnValidate() {
        base.OnValidate();

        if (LocalTreeData.IsNull()) LocalTreeData = new TreeData();
        LocalTreeData.Name = "Tree";
        
        UpdateHealth(MaxHealth);
    }
#endif

    protected override void Awake() {
        base.Awake();

        if (InteractableTrigger.IsNull()) InteractableTrigger = this.GetComponentInHierarchy<InteractableTrigger>();
    }

    public override void Init() {
        LocalTreeData = GetObjectData<TreeData>();
        LocalTreeData.Name = "Tree";
        UpdateHealth(LocalTreeData.Health);

        InteractableTrigger?.Init();

        _ = SpriteFlash(ShaderDataSO.GetSpriteFlashSettings("SPAWN"));
    }

    public void UpdateHealth(float healthAmount) {
        LocalTreeData.Health = healthAmount.Clamp(0, MaxHealth).ToIntFloor();
    }

    public void OnInteract() {
        ChopTree();
    }

    public void OnHoverStart() {
        _ = SpriteFlash(ShaderDataSO.GetSpriteFlashSettings("HOVER"));
    }

    public void OnHoverEnd() {}

    public void ChopTree() {
        UpdateHealth(LocalTreeData.Health - 1);

        if (LocalTreeData.Health <= 0) {
            OnDestroyed();
        }
        else {
            _ = SpriteFlash(ShaderDataSO.GetSpriteFlashSettings("CHOP"));
            ShakeTree();
        }
    }

    public async void ShakeTree() {
        SpriteRenderer.transform.DOComplete();
        SpriteRenderer.transform.DOPunchRotation(
            new Vector3(0, 0, Utils.CoinFlip(-ShaderDataSO.punchRotationAmount, ShaderDataSO.punchRotationAmount)),
            ShaderDataSO.punchRotationDuration,
            ShaderDataSO.punchRotationVibrato,
            ShaderDataSO.punchRotationElasticity);
        await CreateResources(LootTablesDictionary.GetValueOrDefault(ON_INTERACTED));
    }

    public async void OnDestroyed() {
        SpriteRenderer.transform.DOComplete();
        SpriteRenderer.enabled = false;
        InteractableTrigger?.DisableInteractability();
        await CreateResources(LootTablesDictionary.GetValueOrDefault(ON_DESTROYED));
        WorldGenerator.Instance.RemoveObject(LocalTreeData.ObjectPlacementData.OriginalPosition);
    }

    public async Task CreateResources(List<LootTableSO> lootTablesList) {
        if (lootTablesList.IsNullOrEmpty()) {
            Debug.Log($"No Loot Table to draw from!");
            return;
        }

        List<(LootDrop,int)> allLootDropsList = new();
        int totalDrops = 0;

        foreach (LootTableSO lootTable in lootTablesList) {
            List<(LootDrop,int)> lootTableDrops = lootTable.GetDropItems();

            if (lootTableDrops.IsNullOrEmpty()) continue;

            for (int i = 0; i < lootTableDrops.Count; i++) {
                (LootDrop,int) lootDrop = lootTableDrops.GetElement(i);
                int dropAmount = lootDrop.Item2;
                allLootDropsList.Add(lootDrop);
                totalDrops += dropAmount;
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
                    Instantiate(lootDrop.itemPrefab, (UnityEngine.Random.insideUnitCircle * SpawnRadius) + spawnLocation.ToVector2(), Quaternion.identity, WorldGenerator.Instance.worldObjectsParentTransform);
                    
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