using System.Collections.Generic;
using UnityUtilities;
using UnityEngine;
using System.Linq;
using System;
using GD.MinMaxSlider;

// [Serializable]
// public class WeightedRandomList<T> {
//     [Serializable]
//     public struct Pair {
//         public T item;
//         public float weight;

//         public Pair(T item, float weight) {
//             this.item = item;
//             this.weight = weight;
//         }
//     }

//     public List<Pair> list = new List<Pair>();

//     public int Count { get => list.Count; }

//     public T GetRandom() {
//         float totalWeight = 0;

//         if (!list.IsNullOrEmpty()) {
//             totalWeight = list.Sum(pair => pair.weight);
//         }

//         float value = UnityEngine.Random.value * totalWeight;

//         float sumWeight = 0;

//         foreach (Pair pair in list) {
//             sumWeight += pair.weight;

//             if (sumWeight >= value)
//             {
//                 return pair.item;
//             }
//         }

//         return default;
//     }
// }

[CreateAssetMenu(fileName = "NewLootTable", menuName = "Loot System/Loot Table")]
public class LootTableSO : ScriptableObject {
    public bool lootTableEnabled = true;
    
    public List<LootDrop> LootDropsList = new();
    [Min(0)] public int totalWeight = 0;
    bool isInitialized = false;

    [ExecuteInEditMode]
    public void OnValidate() {
        isInitialized = false;
        Initialize();
    }

    public void Initialize() {
        if (!isInitialized) {
            CalculateWeightSum();
            isInitialized = true;
        }
    }
    
    public void CalculateWeightSum() {
        totalWeight = 0;

        if (LootDropsList.IsNullOrEmpty()) {
            return;
        }

        totalWeight = LootDropsList.Sum(item => item.isGuaranteed ? 0 : item.dropWeight);
    }

    public List<(LootDrop,int)> GetDropItems(bool weighted = true) {
        return weighted ? GetWeightedDropItems() : GetUnweightedDropItems();
    }

    List<(LootDrop,int)> GetWeightedDropItems() {
        Initialize();

        int randomDrop = UnityEngine.Random.Range(0, totalWeight);

        List<(LootDrop,int)> droppedLoot = new();
        
        foreach (LootDrop lootDrop in LootDropsList) {
            if (randomDrop <= lootDrop.dropWeight || lootDrop.isGuaranteed) {
                int dropAmount = UnityEngine.Random.Range(lootDrop.dropAmount.x, lootDrop.dropAmount.y + 1);
                droppedLoot.Add((lootDrop , dropAmount));
            }
            else {
                randomDrop -= lootDrop.dropWeight;
            }
        }

        return droppedLoot;
    }

    List<(LootDrop,int)> GetUnweightedDropItems() {
        int randomDrop = 0;

        List<(LootDrop,int)> droppedLoot = new();

        foreach (LootDrop lootDrop in LootDropsList) {
            randomDrop = UnityEngine.Random.Range(0, 100);

            if (randomDrop <= lootDrop.dropWeight || lootDrop.isGuaranteed) {
                int dropAmount = UnityEngine.Random.Range(lootDrop.dropAmount.x, lootDrop.dropAmount.y + 1);
                droppedLoot.Add((lootDrop , dropAmount));
            }
        }

        return droppedLoot;
    }

    // public List<LootDrop> GetDropItems(bool weighted = true) {
    //     Initialize();
    //     return weighted ? GetWeightedDropItems() : GetUnweightedDropItems();
    // }

    // List<LootDrop> GetWeightedDropItems() {
    //     int randomDrop = UnityEngine.Random.Range(0, totalWeight);

    //     List<LootDrop> droppedLoot = new();
        
    //     foreach (LootDrop lootDrop in LootDropsList) {
    //         if (randomDrop <= lootDrop.dropWeight || lootDrop.isGuaranteed) {
    //             droppedLoot.Add(lootDrop);
    //         }
    //         else {
    //             randomDrop -= lootDrop.dropWeight;
    //         }
    //     }

    //     return droppedLoot;
    // }

    // List<LootDrop> GetUnweightedDropItems() {
    //     int randomDrop = 0;

    //     List<LootDrop> droppedLoot = new();

    //     foreach (LootDrop lootDrop in LootDropsList) {
    //         randomDrop = UnityEngine.Random.Range(0, 100);

    //         if (randomDrop <= lootDrop.dropWeight || lootDrop.isGuaranteed) {
    //             droppedLoot.Add(lootDrop);
    //         }
    //     }

    //     return droppedLoot;
    // }
}

[Serializable]
public class LootDrop {
    public GameObject itemPrefab;
    public bool isGuaranteed = false;
    [Range(0, 100)] public int dropWeight = 100;
    [MinMaxSlider(0,100)] public Vector2Int dropAmount = new Vector2Int(0, 1);
}