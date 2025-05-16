using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "NewRegionData", menuName = "Data/Map/Region Data")]
public class RegionData : ScriptableObject {
    public List<TerrainType> RegionsList = new List<TerrainType>();
}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color color;
    public WorldTile worldTile;
}