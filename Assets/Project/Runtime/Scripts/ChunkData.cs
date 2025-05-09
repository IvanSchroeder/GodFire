using UnityEngine;
using System.Collections.Generic;
using System;
using UnityUtilities;
using UnityEngine.Tilemaps;
using UnityEngine.PlayerLoop;

[Serializable]
public class ChunkData {
    public Dictionary<Vector3Int, Chunk> ChunksDictionary = new Dictionary<Vector3Int, Chunk>();
    [NonSerialized] public Dictionary<Vector3Int, Chunk> ModifiedChunksDictionary = new Dictionary<Vector3Int, Chunk>();
    // [NonSerialized] public Dictionary<Vector3Int, Chunk> LoadedChunksDictionary = new Dictionary<Vector3Int, Chunk>();
    // [NonSerialized] public Dictionary<Vector3Int, Chunk> UnloadedChunksDictionary = new Dictionary<Vector3Int, Chunk>();

    public ChunkData() {
    }

    public Chunk[] GetChunksInArea(Vector3Int centerChunkPosition, int xSquareSize = 0, int ySquareSize = 0) {
        Chunk[] surroundingChunks = new Chunk[xSquareSize * ySquareSize];
        Vector3Int checkPos;
        int i = 0;

        for (int y = -ySquareSize; y <= ySquareSize; y++) {
            for (int x = -xSquareSize; x <= xSquareSize; x++) {
                checkPos = new Vector3Int(x, y, 0);
                surroundingChunks[i] = ChunksDictionary.GetValueOrDefault(centerChunkPosition + checkPos, null);
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

    // public Chunk LoadChunk(Vector3Int chunkLocalPos) {
    //     Chunk chunk = GetChunkAt(chunkLocalPos);

    //     if (chunk.IsNotNull()) {
    //         chunk.Load();
    //         UpdateChunkDictionaries(chunk);
    //     }
    //     else {
    //         Debug.Log("Chunk is null!");
    //     }

    //     return chunk;
    // }

    // public void UnloadChunk(Vector3Int chunkLocalPos) {
    //     Chunk chunk = GetChunkAt(chunkLocalPos);
        
    //     if (chunk.IsNull()) return;

    //     chunk.Unload();
    //     // ChunksDictionary.RemoveIfExists(chunkLocalPos);
    //     UpdateChunkDictionaries(chunk);
    // }

    public Chunk UpdateChunk(Vector3Int chunkLocalPos, bool state) {
        Chunk chunk = GetChunkAt(chunkLocalPos);

        if (chunk.IsNotNull()) {
            if (state) {
                chunk.Load();
                UpdateChunkDictionaries(chunk);
            }
            else {
                chunk.Unload();
                // ChunksDictionary.RemoveIfExists(chunkLocalPos);
                UpdateChunkDictionaries(chunk);
            }
        }
        else {
            Debug.Log("Chunk is null!");
        }

        return chunk;
    }

    public void UpdateChunkDictionaries(Chunk chunk) {
        // if (chunk.isLoaded) {
        //     LoadedChunksDictionary.AddIfNotExists(chunk.ID, chunk); 
        //     UnloadedChunksDictionary.RemoveIfExists(chunk.ID);
        // }
        // else {
        //     LoadedChunksDictionary.RemoveIfExists(chunk.ID); 
        //     UnloadedChunksDictionary.AddIfNotExists(chunk.ID, chunk);
        // }
    }

    public void UpdateModifiedChunksDictionary(Chunk chunk) {
        if (CheckIfModified(chunk)) ModifiedChunksDictionary.AddIfNotExists(chunk.LocalPosition, chunk);
    }

    public Chunk GetChunkAt(Vector3Int pos) => ChunksDictionary.GetValueOrDefault(pos);

    public bool CheckIfExists(Vector3Int pos) => ChunksDictionary.ContainsKey(pos);

    public bool CheckIfLoaded(Vector3Int pos) {
        if (!CheckIfExists(pos)) return false;
        return GetChunkAt(pos).isLoaded;
    }
    public bool CheckIfLoaded(Chunk chunk) => chunk.isLoaded;
    
    public bool CheckIfModified(Vector3Int pos) {
        if (!CheckIfExists(pos)) return false;
        return GetChunkAt(pos).isModified;
    }
    public bool CheckIfModified(Chunk chunk) => chunk.isModified;
}
