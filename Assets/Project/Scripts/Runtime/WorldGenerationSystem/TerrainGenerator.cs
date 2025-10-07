using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityUtilities;
using Random = UnityEngine.Random;

public class TerrainGenerator : MonoBehaviour {
    public WorldTile defaultTile;
    [Range(0, 100)] public int treeSpawnChance = 50;
    [Range(0, 100)] public int treeRespawnChance = 50;

    public List<WorldTile> TilesList = new();
    public Dictionary<int, WorldTile> WorldTilesIDsDictionary = new();
    public Dictionary<WorldTile, int> WorldTilesCountDictionary = new();

    private void Awake() {
        Init();
    }

    public void Init() {
        WorldTilesIDsDictionary = new();
        WorldTilesCountDictionary = new();

        foreach (WorldTile tile in TilesList) {
            WorldTilesIDsDictionary.AddIfNotExists(tile.id, tile);
            WorldTilesCountDictionary.AddIfNotExists(tile, 0);
        }
    }

    public Chunk ProcessChunkColumn(Chunk chunk, int x, int y, HeightMapSettings heightMapSettings) {
        WorldTile worldTile = DetermineWorldTile(chunk.GetLocalNoiseValueAt(x, y), heightMapSettings);
        WorldTilesCountDictionary.AddOrReplace(worldTile, WorldTilesCountDictionary.GetValueOrDefault(defaultTile) + 1);
        WorldGenerator.Instance.ChunkData.terrainIDsMap[chunk.GridPosition.x + x,chunk.GridPosition.y + y] = worldTile.id;
        
        chunk.SaveTile(new Vector3Int(x, y, 0), worldTile);
        
        return chunk;
    }

    private WorldTile DetermineWorldTile(float heightValue, HeightMapSettings heightMapSettings) {
        WorldTile worldTile = defaultTile;
        float previousRangeLimit = 1;

        for (int i = heightMapSettings.RegionData.RegionsList.Count - 1; i >= 0; i--) {
            if (heightValue <= previousRangeLimit) {
                worldTile = heightMapSettings.RegionData.RegionsList.GetElement(i).worldTile;
                previousRangeLimit = heightMapSettings.RegionData.RegionsList.GetElement(i).height;
            }
            else {
                break;
            }
        }

        return worldTile;
    }

    private int DetermineWorldTileID(float heightValue, HeightMapSettings heightMapSettings) {
        WorldTile worldTile = defaultTile;
        float previousRangeLimit = 1;

        for (int i = heightMapSettings.RegionData.RegionsList.Count - 1; i >= 0; i--) {
            if (heightValue <= previousRangeLimit) {
                worldTile = heightMapSettings.RegionData.RegionsList.GetElement(i).worldTile;
                previousRangeLimit = heightMapSettings.RegionData.RegionsList.GetElement(i).height;
            }
            else {
                break;
            }
        }

        return worldTile.id;
    }

    public Chunk GenerateTerrainChunk(Chunk chunk, HeightMap worldHeightMap, HeightMapSettings heightMapSettings) {
        float[,] chunkTerrainValues = HeightMapGenerator.GetMapValuesAt(worldHeightMap.values, chunk.GridPosition.ToVector2Int(), chunk.chunkSize, chunk.chunkSize);
        chunk.localNoiseValues = chunkTerrainValues;

        for (int y = 0; y < chunk.chunkSize; y++) {
            for (int x = 0; x < chunk.chunkSize; x++) {
                chunk = ProcessChunkColumn(chunk, x, y, heightMapSettings);
            }
        }
        
        return chunk;
    }

    public float[,] GenerateTreesMap(float[,] worldTerrainMap, HeightMapSettings heightMapSettings) {
        float[,] worldTreesMap = HeightMapGenerator.GetMapValuesInRange(worldTerrainMap,
                heightMapSettings.GetTerrainTypeByTileType(TileType.DeepGrass).height, heightMapSettings.GetTerrainTypeByTileType(TileType.Rock).height,
                minInclusive: true, maxInclusive: true, normalized: true);

        for (int y = 0; y < worldTreesMap.GetLength(1); y++) {
            for (int x = 0; x < worldTreesMap.GetLength(0); x++) {
                if (worldTreesMap[x,y] <= 0) {
                    worldTreesMap[x,y] = 0;
                }
                else {
                    worldTreesMap[x,y] = 1;
                }

                // int spawnChance = Random.Range(0, 100);

                // if (treeSpawnChance >= spawnChance) {
                //     worldTreesMap[x,y] = 1;
                // }
                // else {
                //     worldTreesMap[x,y] = 0;
                // }
            }
        }

        return worldTreesMap;
    }

    public async UniTask PlaceTrees(Chunk chunk, float[,] worldTreesMapValues, ObjectData objectData, GridData gridData, int delayMilliseconds, bool firstSpawning) {
        Vector3Int gridPosition;

        float[,] localTreesMapValues = HeightMapGenerator.GetMapValuesAt(worldTreesMapValues,
                    chunk.GridPosition.ToVector2Int(),
                    chunk.chunkSize,
                    chunk.chunkSize);

        for (int y = 0; y < chunk.chunkSize; y++) {
            for (int x = 0; x < chunk.chunkSize; x++) {
                gridPosition = chunk.WorldPosition.Add(x, y);

                if (localTreesMapValues[x,y] <= 0 || !gridData.CanPlaceObjectAt(gridPosition, objectData.Size)) continue;

                int spawnChance = Random.Range(0, 100);

                if ((firstSpawning ? treeSpawnChance : treeRespawnChance) < spawnChance) continue;

                WorldGenerator.Instance.PlaceObjectAt(objectData.Prefab, gridPosition, objectData, new TreeData());
                if (!firstSpawning) AudioManager.Instance.PlaySFX("Pop", false);

                await UniTask.Delay(delayMilliseconds, true);
            }
        }

        if (firstSpawning) AudioManager.Instance.PlaySFX("Pop", false);

        await UniTask.Yield();
    }
}