using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;
using UnityUtilities;

[Serializable]
public class Chunk {
    Vector3Int _localPosition;
    Vector3Int _gridPosition;
    Vector3Int _worldPosition;
    Vector3 _centerPosition;
    BoundsInt chunkBounds;
    public int chunkSize = 16;
    public bool isLoaded = false;
    public bool isModified = false;
    // public CustomTile[] tiles = new CustomTile[0];
    public Dictionary<Vector3Int, WorldTile> TilesInChunkDictionary;
    public Dictionary<Vector3Int, WorldTile> ObjectsInChunkDictionary;
    public Dictionary<Vector3Int, Vector3Int> TilesInWorldGridPositionsDictionary;
    public Tilemap groundTilemap;
    public Tilemap waterTilemap;
    public float[,] noiseValues = new float[0,0];
    TileBase[] nullArray;
    TileBase[] wArray;
    TileBase[] gArray;

    public Vector3Int LocalPosition {
        get {
            return _localPosition;
        }
        set {
            _localPosition = value;
        }
    }

    public Vector3Int GridPosition {
        get {
            return _gridPosition;
        }
        set {
            _gridPosition = value;
        }
    }

    public Vector3Int WorldPosition {
        get {
            return _worldPosition;
        }
        set {
            _worldPosition = value;
        }
    }

    public Vector3 CenterPosition {
        get {
            return _centerPosition;
        }
        set {
            _centerPosition = value;
        }
    }

    public Chunk(Vector3Int _localPos, Vector3Int _gridPos, Vector3Int _worldPos, int _size, Tilemap _groundTilemap, Tilemap _waterTilemap) {
        Initialize(_localPos, _gridPos, _worldPos, _size, _groundTilemap, _waterTilemap);
    }

    public void Initialize(Vector3Int _localPos, Vector3Int _gridPos, Vector3Int _worldPos, int _size, Tilemap _groundTilemap, Tilemap _waterTilemap) {
        _localPosition = _localPos;
        _gridPosition = _gridPos;
        _worldPosition = _worldPos;
        _centerPosition = new Vector3(_worldPosition.x + (_size / 2), _worldPosition.y + (_size / 2), 0);
        chunkSize = _size;

        TilesInChunkDictionary = new Dictionary<Vector3Int, WorldTile>();
        TilesInWorldGridPositionsDictionary = new Dictionary<Vector3Int, Vector3Int>();

        chunkBounds = new BoundsInt(_worldPosition, new Vector3Int(chunkSize, chunkSize, 1));
        // tiles = new CustomTile[chunkSize * chunkSize];
        isLoaded = true;

        noiseValues = new float[chunkSize,chunkSize];

        groundTilemap = _groundTilemap;
        waterTilemap = _waterTilemap;

        nullArray = new TileBase[chunkSize * chunkSize];
    }

    public void Load() {
        if (isLoaded) return;

        wArray = new TileBase[chunkSize * chunkSize];
        gArray = new TileBase[chunkSize * chunkSize];

        Vector3Int tileLocal;
        int i = 0;

        for (int y = 0; y < chunkSize; y++) {
            for (int x = 0; x < chunkSize; x++) {
                tileLocal = new Vector3Int(x, y, 0);
                WorldTile customTile = TilesInChunkDictionary.GetValueOrDefault(tileLocal);

                switch (customTile.targetTilemap) {
                    case TargetTilemap.Water:
                        wArray[i] = customTile.tile;
                        gArray[i] = null;
                        break;
                    case TargetTilemap.Ground:
                        wArray[i] = null;
                        gArray[i] = customTile.tile;
                        break;
                }

                i++;
            }
        }

        waterTilemap.SetTilesBlock(chunkBounds, wArray);
        groundTilemap.SetTilesBlock(chunkBounds, gArray);

        isLoaded = true;
    }

    public void Unload() {
        if (!isLoaded) return;

        waterTilemap.SetTilesBlock(chunkBounds, nullArray);
        groundTilemap.SetTilesBlock(chunkBounds, nullArray);

        isLoaded = false;
    }

    // public void LoopThroughTiles(Action<int, int, int> actionToPerform) {
    //     for (int index = 0; index < tiles.Length; index++) {
    //         var position = GetPositionFromIndex(index);
    //         actionToPerform(position.x, position.y, position.z);
    //     }
    // }

    Vector3Int GetPositionFromIndex(int index) {
        int x = index % chunkSize;
        int y = index / (chunkSize * chunkSize);
        // int z = (index / chunkSize) % chunkSize;
        int z = 0;
        return new Vector3Int(x, y, z);
    }

    int GetIndexFromPosition(int x, int y) {
        return (x * chunkSize) + y;
    }

    bool InRange(int axisCoordinate) {
        if (axisCoordinate < 0 || axisCoordinate >= chunkSize) {
            return false;
        }

        return true;
    }

    public Vector3Int GetTileInChunkCoordinates(Vector3Int pos) {
        return new Vector3Int() {
            x = pos.x - _worldPosition.x,
            y = pos.y - _worldPosition.y,
            z = pos.z - _worldPosition.z
        };
    }

    public Vector3Int GetTileInChunkCoordinates(int x2, int y2, int z2) {
        return new Vector3Int() {
            x = x2 - _worldPosition.x,
            y = y2 - _worldPosition.y,
            z = z2 - _worldPosition.z
        };
    }

    public WorldTile GetTileFromWorldCoordinates(int x, int y, int z = 0) {
        return TilesInChunkDictionary.GetValueOrDefault(GetTileCoordinatesFromWorldCoordinates(new Vector3Int(x, y, z)));
    }

    public WorldTile GetTileFromWorldCoordinates(Vector3 position) {
        return TilesInChunkDictionary.GetValueOrDefault(GetTileCoordinatesFromWorldCoordinates(position));
    }

    public WorldTile GetTileFromWorldCoordinates(Vector3Int position) {
        return TilesInChunkDictionary.GetValueOrDefault(GetTileCoordinatesFromWorldCoordinates(position));
    }

    public Vector3Int GetTileCoordinatesFromWorldCoordinates(Vector3 tileWorldCoordinates) {
        return TilesInWorldGridPositionsDictionary.GetValueOrDefault(tileWorldCoordinates.ToVector3IntFloor());
    }

    public Vector3Int GetTileCoordinatesFromWorldCoordinates(Vector3Int tileWorldCoordinates) {
        return TilesInWorldGridPositionsDictionary.GetValueOrDefault(tileWorldCoordinates);
    }

    public float GetNoiseValueAt(Vector3Int tileCoordinates) {
        return noiseValues[tileCoordinates.x, tileCoordinates.y];
    }

    public float GetNoiseValueAt(Vector3 tileWorldCoordinates) {
        return GetNoiseValueAt(GetTileCoordinatesFromWorldCoordinates(tileWorldCoordinates));
    }

    // public CustomTile GetTileFromChunkCoordinates(int x, int y, int z = 0) {
    //     if (InRange(x) && InRange(y)) {
    //         int index = GetIndexFromPosition(x, y);
    //         return tiles[index];
    //     }

    //     throw new Exception("Tile not found");
    // }

    // public CustomTile GetTileFromChunkCoordinates(Vector3Int chunkCoordinates) {
    //     return GetTileFromChunkCoordinates(chunkCoordinates.x, chunkCoordinates.y, chunkCoordinates.z);
    // }

    public void SetTile(Vector3Int _localPosition, WorldTile customTile, float noiseValue) {
        if (InRange(_localPosition.x) && InRange(_localPosition.y)) {
            // int index = GetIndexFromPosition(_localPosition.x, _localPosition.y);
            // tiles[index] = customTile;
            Vector3Int tilePosInWorldGrid = _worldPosition + _localPosition;
            TilesInChunkDictionary.AddOrReplace(_localPosition, customTile);
            TilesInWorldGridPositionsDictionary.AddOrReplace(tilePosInWorldGrid, _localPosition);
            noiseValues[_localPosition.x, _localPosition.y] = noiseValue;

            PaintTile(customTile, tilePosInWorldGrid);
        }
    }

    public void PaintTile(WorldTile customTile, Vector3Int tilePosInWorldGrid) {
        TileBase tileBase = customTile.tile;
        
        switch (customTile.targetTilemap) {
            case TargetTilemap.Water:
                GenerateTile(waterTilemap, tileBase, tilePosInWorldGrid);
                GenerateTile(groundTilemap, null, tilePosInWorldGrid);
                break;
            case TargetTilemap.Ground:
                GenerateTile(waterTilemap, null, tilePosInWorldGrid);
                GenerateTile(groundTilemap, tileBase, tilePosInWorldGrid);
                break;
        }
    }

    public void GenerateTile(Tilemap _tilemap, TileBase _tile, Vector3Int _pos) {
        _tilemap.SetTile(_pos, _tile);
    }

    public void GenerateTiles(Tilemap _tilemap, TileBase[] _tiles, Vector3Int[] _pos) {
        _tilemap.SetTiles(_pos, _tiles);
    }

    public void GenerateTile(Tilemap[] _tilemaps, TileBase _tile, Vector3Int _pos) {
        foreach (Tilemap tilemap in _tilemaps) {
            tilemap.SetTile(_pos, _tile);
        }
    }

    public void GenerateTiles(Tilemap[] _tilemaps, TileBase[] _tiles, Vector3Int[] _pos) {
        foreach (Tilemap tilemap in _tilemaps) {
            tilemap.SetTiles(_pos, _tiles);
        }
    }

    public void ModifyChunk() {
        if (!isModified) isModified = true;
    }
}

[Serializable]
public class GridObject {

}