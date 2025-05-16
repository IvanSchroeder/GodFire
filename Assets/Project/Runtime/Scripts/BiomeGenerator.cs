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
    Dictionary<TileType, WorldTile> WorldTilesDictionary = new();
    public SerializedDictionary<WorldTile, int> WorldTilesCountDictionary = new();

    void Awake() {
        WorldTilesDictionary = new();
        WorldTilesCountDictionary = new();

        foreach (WorldTile tile in TilesList) {
            WorldTilesDictionary.AddIfNotExists(tile.type, tile);
            WorldTilesCountDictionary.AddIfNotExists(tile, 0);
        }
    }

    public Chunk ProcessChunkColumn(Chunk chunk, int x, int y, float heightValue, HeightMapSettings heightMapSettings) {
        WorldTile customTile = DetermineWorldTile(heightValue, heightMapSettings);
        
        chunk.SaveTile(new Vector3Int(x, y, 0), customTile, heightValue);
        
        return chunk;
    }

    WorldTile DetermineWorldTile(float heightValue, HeightMapSettings heightMapSettings) {
        for (int i = 0; i < heightMapSettings.regionData.RegionsList.Count; i++) {
            if (heightValue <= heightMapSettings.regionData.RegionsList.GetElement(i).height) {
                WorldTile worldTile = heightMapSettings.regionData.RegionsList.GetElement(i).worldTile;
                WorldTilesCountDictionary.AddOrReplace(worldTile, WorldTilesCountDictionary.GetValueOrDefault(worldTile) + 1);
                return worldTile;
            }
            else {
                continue;
            }
        }

        return defaultTile;
    }
}