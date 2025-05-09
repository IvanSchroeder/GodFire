using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType {
    DeepWater,
    Water,
    Sand,
    Dirt,
    Grass,
    DeepGrass,
    Rock,
    Rock2
}

public enum TargetTilemap {
    Water,
    Lava,
    Ground,
}

[CreateAssetMenu(fileName = "NewWorldTile", menuName = "WorldEditor/World Tile")]
public class WorldTile : ScriptableObject {
    public TileBase tile;
    public string id;
    public TileType type = TileType.Dirt;
    public TargetTilemap targetTilemap = TargetTilemap.Ground;
}