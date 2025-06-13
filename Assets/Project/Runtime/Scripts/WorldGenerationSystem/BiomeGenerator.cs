using System.Collections.Generic;
using System.Linq;
using UnityUtilities;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class BiomeGenerator : MonoBehaviour {
    public NoiseSettings biomeNoiseSettings;
    public WorldTile defaultTile;

    public List<WorldTile> TilesList = new();
    public Dictionary<int, WorldTile> WorldTilesIDsDictionary = new();
    public Dictionary<WorldTile, int> WorldTilesCountDictionary = new();

    void Awake() {
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

    WorldTile DetermineWorldTile(float heightValue, HeightMapSettings heightMapSettings) {
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

            // if (heightValue <= heightMapSettings.RegionData.RegionsList.GetElement(i).height) {
            //     terrainColor = heightMapSettings.RegionData.RegionsList.GetElement(i).worldTile.color;
            // }
            // else {
            //     continue;
            // }
        }

        return worldTile;
    }
}