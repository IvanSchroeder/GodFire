using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "NewHeightMapSettings", menuName = "Data/Map/Height Map Settings")]
public class HeightMapSettings : ScriptableObject {
    [Header("Noise Parameters")]
    [Space(5f)]
    public NoiseSettings noiseSettings;
    public RegionData regionData;

    [Space(10f)]
    [Header("Falloff Parameters")]
    [Space(5f)]
    public FalloffSettings falloffSettings;

    #if UNITY_EDITOR
    void OnValidate() {
        noiseSettings.ValidateValues();
    }
    #endif

    public TerrainType GetTerrainTypeByTile(TileType tileType) {
        return regionData.RegionsList.FirstOrDefault(t => t.worldTile.type == tileType);
    } 
}
