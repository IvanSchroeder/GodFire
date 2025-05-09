using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRegionData", menuName = "Data/Map/Region Data")]
public class RegionData : ScriptableObject {
    public List<TerrainType> RegionsList = new List<TerrainType>();
}