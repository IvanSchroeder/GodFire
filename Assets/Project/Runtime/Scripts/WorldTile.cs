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
    public string tileName = "New World Tile";
    public int id = 0;
    public TileBase tile;
    public TileType type = TileType.DeepWater;
    public TargetTilemap targetTilemap = TargetTilemap.Water;
    public Color color = Color.white;
}