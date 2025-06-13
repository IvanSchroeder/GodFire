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
    BoundsInt _chunkBounds;

    public int chunkSize = 16;
    [NonSerialized] public bool isLoaded = false;
    [NonSerialized] public bool firstTimeLoading = true;
    public bool isModified = false;
    [NonSerialized] public Tilemap groundTilemap;
    [NonSerialized] public Tilemap waterTilemap;

    public Dictionary<Vector3Int, WorldTile> TilesInChunkDictionary;
    public Dictionary<Vector3Int, Vector3Int> TilesInWorldGridPositionsDictionary;
    public float[,] localNoiseValues;

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

        TilesInChunkDictionary = new();
        TilesInWorldGridPositionsDictionary = new();

        _chunkBounds = new BoundsInt(_worldPosition, new Vector3Int(chunkSize, chunkSize, 1));
        // tiles = new CustomTile[chunkSize * chunkSize];
        isLoaded = true;

        localNoiseValues = new float[chunkSize,chunkSize];

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

        waterTilemap.SetTilesBlock(_chunkBounds, wArray);
        groundTilemap.SetTilesBlock(_chunkBounds, gArray);

        isLoaded = true;
    }

    public void Unload() {
        if (!isLoaded) return;

        waterTilemap.SetTilesBlock(_chunkBounds, nullArray);
        groundTilemap.SetTilesBlock(_chunkBounds, nullArray);

        isLoaded = false;
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

    public Vector3Int GetTileInChunkCoordinates(int _x, int _y, int _z) {
        return new Vector3Int() {
            x = _x - _worldPosition.x,
            y = _y - _worldPosition.y,
            z = _z - _worldPosition.z
        };
    }

    public WorldTile GetTileFromWorldCoordinates(int x, int y, int z = 0) {
        return TilesInChunkDictionary.GetValueOrDefault(GetTileCoordinatesFromWorldCoordinates(new Vector3Int(x, y, z)));
    }

    public WorldTile GetTileFromWorldCoordinates(Vector3 tileWorldCoordinates) {
        return TilesInChunkDictionary.GetValueOrDefault(GetTileCoordinatesFromWorldCoordinates(tileWorldCoordinates));
    }

    public WorldTile GetTileFromWorldCoordinates(Vector3Int tileWorldCoordinates) {
        return TilesInChunkDictionary.GetValueOrDefault(GetTileCoordinatesFromWorldCoordinates(tileWorldCoordinates));
    }

    public Vector3Int GetTileCoordinatesFromWorldCoordinates(Vector3 tileWorldCoordinates) {
        return TilesInWorldGridPositionsDictionary.GetValueOrDefault(tileWorldCoordinates.ToVector3IntFloor());
    }

    public Vector3Int GetTileCoordinatesFromWorldCoordinates(Vector3Int tileWorldCoordinates) {
        return TilesInWorldGridPositionsDictionary.GetValueOrDefault(tileWorldCoordinates);
    }

    public float[,] GetLocalNoiseValues() => localNoiseValues;
    public float GetLocalNoiseValueAt(Vector2Int tileCoordinates) => localNoiseValues[tileCoordinates.x, tileCoordinates.y];
    public float GetLocalNoiseValueAt(int x, int y) => localNoiseValues[x, y];
    public float GetLocalNoiseValueAt(Vector3 tileWorldCoordinates) => GetLocalNoiseValueAt(GetTileCoordinatesFromWorldCoordinates(tileWorldCoordinates));

    public void SaveTile(Vector3Int _localPosition, WorldTile customTile) {
        if (InRange(_localPosition.x) && InRange(_localPosition.y)) {
            Vector3Int tilePosInWorldGrid = _worldPosition + _localPosition;
            TilesInChunkDictionary.AddOrReplace(_localPosition, customTile);
            TilesInWorldGridPositionsDictionary.AddOrReplace(tilePosInWorldGrid, _localPosition);

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