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
        float[,] chunkHeightValues = HeightMapGenerator.GetMapValuesAt(worldHeightMap.values, chunk.GridPosition.ToVector2Int(), chunk.chunkSize);

        for (int y = 0; y < chunk.chunkSize; y++) {
            for (int x = 0; x < chunk.chunkSize; x++) {
                chunk = biomeGenerator.ProcessChunkColumn(chunk, x, y, chunkHeightValues[x,y], heightMapSettings);
            }
        }
        
        return chunk;
    }

    public Chunk GenerateTrees(Chunk chunk, HeightMap worldHeightMap, HeightMapSettings heightMapSettings, GridData gridData, WorldObjectData objectData, GridLayout grid, float yOffset) {
        float[,] chunkHeightValues = HeightMapGenerator.GetMapValuesAt(worldHeightMap.values, chunk.GridPosition.ToVector2Int(), chunk.chunkSize);

        float[,] treeMapValues = HeightMapGenerator.GetMapValuesInRange(chunkHeightValues,
            heightMapSettings.GetTerrainTypeByTile(TileType.Grass).height, heightMapSettings.GetTerrainTypeByTile(TileType.DeepGrass).height,
            minInclusive: true, maxInclusive: true, normalized: true);

        // Random.InitState(heightMapSettings.noiseSettings.seed);
        Vector3Int gridPosition;

        for (int y = 0; y < chunk.chunkSize; y++) {
            for (int x = 0; x < chunk.chunkSize; x++) {
                gridPosition = chunk.WorldPosition.Add(x, y);

                if (treeMapValues[x,y] > 0 && gridData.CanPlaceObjectAt(gridPosition, objectData.Size)) {
                    int spawnChance = Random.Range(0, 100);

                    if (treeSpawnChance >= spawnChance) {
                        int objectListIndex = ObjectPlacer.Instance.PlaceObjectAt(objectData.Prefab, grid.CellToWorld(gridPosition), objectData.Size.y * yOffset);
                        gridData.AddObjectAt(gridPosition, objectData.Size, objectData.ID, objectListIndex);
                        treesCount++;
                    }
                }
            }
        }

        Debug.Log($"Trees Count: {treesCount}");

        return chunk;
    }
}

public class TreeData {

}