using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewChunkManagementSettings", menuName = "Data/Map/Chunk Management Settings")]
public class ChunkManagementSettings : ScriptableObject {
    [Range(1, 100)] public int worldWidthInChunks = 51;
    [Range(1, 100)] public int worldHeightInChunks = 51;
    [Range(1, 128)] public int chunkSize = 16;
    [Range(1, 30)] public int chunkRenderDistance = 2;
    public int chunkUnloadingDistance = 1;
    [Range(0, 10)] public int chunkActiveThreshold = 2;
    [Range(1, 10)] public int chunksMovedThreshold = 1;
    [Range(1, 100)] public int chunksLoadedPerFrame = 10;
    [Range(1, 100)] public int loadInterval = 2;
    [Range(1, 100)] public int chunksUnloadedPerFrame = 50;
    [Range(1, 100)] public int unloadInterval = 1;

    #if UNITY_EDITOR
    void OnValidate() {
        ValidateValues();
    }
    #endif

    public void ValidateValues() {
        chunkUnloadingDistance = chunkRenderDistance + chunkActiveThreshold;
    }
}
