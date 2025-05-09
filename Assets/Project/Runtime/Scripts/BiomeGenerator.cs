using System.Collections.Generic;
using System.Linq;
using UnityUtilities;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BiomeGenerator : MonoBehaviour {
    public NoiseSettings biomeNoiseSettings;
    public DomainWarping domainWarping;
    public WorldTile defaultTile;

    public List<WorldTile> TilesList = new List<WorldTile>();
    Dictionary<TileType, WorldTile> WorldTilesDictionary = new Dictionary<TileType, WorldTile>();

    void Awake() {
        WorldTilesDictionary = new Dictionary<TileType, WorldTile>();

        foreach (WorldTile tile in TilesList) {
            WorldTilesDictionary.AddIfNotExists(tile.type, tile);
        }
    }

    public Chunk ProcessChunkColumn(Chunk chunk, int x, int y, float heightValue, HeightMapSettings heightMapSettings) {
        WorldTile customTile = DetermineWorldTile(heightValue, heightMapSettings);
        
        chunk.SetTile(new Vector3Int(x, y, 0), customTile, heightValue);
        return chunk;
    }

    WorldTile DetermineWorldTile(float heightValue, HeightMapSettings heightMapSettings) {
        for (int i = 0; i < heightMapSettings.regionData.RegionsList.Count; i++) {
            if (heightValue <= heightMapSettings.regionData.RegionsList.GetElement(i).height) {
                return heightMapSettings.regionData.RegionsList.GetElement(i).worldTile;
            }
            else {
                continue;
            }
        }

        return defaultTile;
    }
}