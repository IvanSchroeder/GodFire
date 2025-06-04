using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "NewRegionData", menuName = "Data/Map/Region Data")]
public class RegionData : ScriptableObject {
    public List<TerrainType> RegionsList = new List<TerrainType>();

    public TerrainType defaultTerrain = new();
}

[Serializable]
public struct TerrainType {
    public float height;
    public WorldTile worldTile;
}