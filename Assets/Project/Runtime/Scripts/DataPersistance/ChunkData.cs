using UnityEngine;
using System.Collections.Generic;
using System;
using UnityUtilities;
using UnityEngine.Tilemaps;
using Newtonsoft.Json;

[Serializable]
public class ChunkData {
    public Dictionary<Vector3Int, Chunk> ChunksDictionary = new Dictionary<Vector3Int, Chunk>();
    public Dictionary<Vector3Int, Chunk> ModifiedChunksDictionary = new Dictionary<Vector3Int, Chunk>();

    public ChunkData() {}

    public Chunk[] GetChunksInArea(Vector3Int centerChunkPosition, int xSquareSize = 0, int ySquareSize = 0) {
        Chunk[] surroundingChunks = new Chunk[xSquareSize * ySquareSize];
        Vector3Int checkPos;
        int i = 0;

        for (int y = -ySquareSize; y <= ySquareSize; y++) {
            for (int x = -xSquareSize; x <= xSquareSize; x++) {
                checkPos = new Vector3Int(x, y, 0);
                surroundingChunks[i] = GetChunkAt(centerChunkPosition + checkPos);
                i++;
            }
        }
        return surroundingChunks;
    }

    public Chunk CreateChunk(Chunk chunk, Vector3Int localPos, Vector3Int gridPos, int chunkSize, Tilemap ground, Tilemap water) {
        chunk = new Chunk(
            localPos,
            gridPos,
            new Vector3Int(localPos.x * chunkSize, localPos.y * chunkSize, 0),
            chunkSize,
            ground,
            water);
        ChunksDictionary.AddIfNotExists(chunk.LocalPosition, chunk);
        return chunk;
    }

    public Chunk UpdateChunk(Vector3Int chunkLocalPos, bool state) {
        Chunk chunk = GetChunkAt(chunkLocalPos);

        if (chunk.IsNotNull()) {
            if (state) {
                chunk.Load();
            }
            else {
                chunk.Unload();
                // ChunksDictionary.RemoveIfExists(chunkLocalPos);
            }
        }
        else {
            Debug.Log("Chunk is null!");
            return null;
        }

        return chunk;
    }

    public Chunk GetChunkAt(Vector3Int chunkLocalPos) => ChunksDictionary.GetValueOrDefault(chunkLocalPos, null);

    public void UpdateModifiedChunksDictionary(Chunk chunk) {
        if (CheckIfModified(chunk)) ModifiedChunksDictionary.AddIfNotExists(chunk.LocalPosition, chunk);
    }

    public bool CheckIfExists(Vector3Int chunkLocalPos) => ChunksDictionary.ContainsKey(chunkLocalPos);

    public bool CheckIfLoaded(Vector3Int chunkLocalPos) {
        if (!CheckIfExists(chunkLocalPos)) return false;
        return GetChunkAt(chunkLocalPos).isLoaded;
    }

    public bool CheckIfLoaded(Chunk chunk) => chunk.isLoaded;
    
    public bool CheckIfModified(Vector3Int chunkLocalPos) {
        if (!CheckIfExists(chunkLocalPos)) return false;
        return GetChunkAt(chunkLocalPos).isModified;
    }
    public bool CheckIfModified(Chunk chunk) => chunk.isModified;
}