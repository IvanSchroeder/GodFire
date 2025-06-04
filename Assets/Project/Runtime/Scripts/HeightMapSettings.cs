using UnityEngine;
using System.Linq;
using UnityUtilities;

[CreateAssetMenu(fileName = "NewHeightMapSettings", menuName = "Data/Map/Height Map Settings")]
public class HeightMapSettings : ScriptableObject {
    [Header("Noise Parameters")]
    [Space(5f)]
    public NoiseSettings NoiseSettings;
    public RegionData RegionData;

    [Space(10f)]
    [Header("Falloff Parameters")]
    [Space(5f)]
    public FalloffSettings FalloffSettings;

    #if UNITY_EDITOR
    void OnValidate() {
        NoiseSettings.ValidateValues();
    }
    #endif

    public TerrainType GetTerrainTypeByTileType(TileType tileType) {
        return RegionData.RegionsList.FirstOrDefault(t => t.worldTile.type == tileType);
    }

    public WorldTile DetermineWorldTile(float heightValue, HeightMapSettings heightMapSettings) {
        WorldTile worldTile = RegionData.RegionsList[0].worldTile;

        for (int i = heightMapSettings.RegionData.RegionsList.Count - 1; i >= 0; i--) {
            worldTile = RegionData.RegionsList[0].worldTile;

            if (heightValue <= heightMapSettings.RegionData.RegionsList.GetElement(i).height) {
                worldTile = heightMapSettings.RegionData.RegionsList.GetElement(i).worldTile;
            }
            else {
                continue;
            }
        }
        
        return worldTile;
    }
}
