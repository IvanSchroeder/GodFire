using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityUtilities;

public class TerrainGenerator : MonoBehaviour {
    public BiomeGenerator biomeGenerator;

    public Chunk GenerateTerrainChunk(Chunk chunk, HeightMap heightMap, HeightMapSettings heightMapSettings) {
        float[,] chunkHeightValues = HeightMapGenerator.GetMapValuesAt(heightMap.values, chunk.GridPosition.ToVector2Int(), chunk.chunkSize);
        float heightValue = 0;

        for (int y = 0; y < chunk.chunkSize; y++) {
            for (int x = 0; x < chunk.chunkSize; x++) {
                heightValue = chunkHeightValues[x,y];
                chunk = biomeGenerator.ProcessChunkColumn(chunk, x, y, heightValue, heightMapSettings);
            }
        }
        
        return chunk;
    }
}