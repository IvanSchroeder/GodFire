using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityUtilities;
using Random = UnityEngine.Random;

public class TerrainGenerator : MonoBehaviour {
    public BiomeGenerator biomeGenerator;

    int treesCount = 0;
    [Range(0, 100)] public int treeSpawnChance = 50;

    public Chunk GenerateTerrainChunk(Chunk chunk, HeightMap worldHeightMap, HeightMapSettings heightMapSettings) {
        float[,] chunkTerrainValues = HeightMapGenerator.GetMapValuesAt(worldHeightMap.values, chunk.GridPosition.ToVector2Int(), chunk.chunkSize, chunk.chunkSize);
        chunk.localNoiseValues = chunkTerrainValues;

        for (int y = 0; y < chunk.chunkSize; y++) {
            for (int x = 0; x < chunk.chunkSize; x++) {
                chunk = biomeGenerator.ProcessChunkColumn(chunk, x, y, heightMapSettings);
            }
        }
        
        return chunk;
    }

    public float[,] GenerateWorldTreesMap(float[,] worldTerrainMap, HeightMapSettings heightMapSettings) {
        float[,] worldTreesMap = HeightMapGenerator.GetMapValuesInRange(worldTerrainMap,
                heightMapSettings.GetTerrainTypeByTileType(TileType.DeepGrass).height, heightMapSettings.GetTerrainTypeByTileType(TileType.Rock).height,
                minInclusive: true, maxInclusive: true, normalized: true);

        for (int y = 0; y < worldTreesMap.GetLength(1); y++) {
            for (int x = 0; x < worldTreesMap.GetLength(0); x++) {
                if (worldTreesMap[x,y] <= 0) {
                    worldTreesMap[x,y] = 0;
                    continue;
                }

                int spawnChance = Random.Range(0, 100);

                if (treeSpawnChance >= spawnChance) {
                    worldTreesMap[x,y] = 1;
                    treesCount++;
                }
                else {
                    worldTreesMap[x,y] = 0;
                }
            }
        }

        return worldTreesMap;
    }

    public Chunk PlaceTrees(Chunk chunk, float[,] worldTreesMapValues, ObjectData objectData, GridData gridData) {
        Vector3Int gridPosition;

        float[,] localTreesMapValues = HeightMapGenerator.GetMapValuesAt(worldTreesMapValues,
                    chunk.GridPosition.ToVector2Int(),
                    chunk.chunkSize,
                    chunk.chunkSize);

        for (int y = 0; y < chunk.chunkSize; y++) {
            for (int x = 0; x < chunk.chunkSize; x++) {
                gridPosition = chunk.WorldPosition.Add(x, y);

                if (localTreesMapValues[x,y] <= 0 || !gridData.CanPlaceObjectAt(gridPosition, objectData.Size)) continue;

                WorldGenerator.Instance.PlaceObjectAt(objectData.Prefab, gridPosition, objectData);
                
                treesCount++;
            }
        }

        Debug.Log($"Placed Trees Count: {treesCount}");

        return chunk;
    }
}