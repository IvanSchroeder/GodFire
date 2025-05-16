using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityUtilities;
using System;
using DG.Tweening;
using System.Collections;
using System.Diagnostics;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;
using System.Linq;
using Random = UnityEngine.Random;
using SaveSystem;
using WorldSimulation;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class WorldGenerator : Singleton<WorldGenerator>, IDataHandler {
#region Variables
    [Header("References")]
    [Space(5f)]
    public TerrainGenerator terrainGenerator;
    public MapPreview mapPreview;
    [HideInInspector] public WorldData WorldData;
    [HideInInspector] public ChunkData ChunkData;
    [HideInInspector] public HeightMap HeightMap;
    public GridData GridData;
    [SerializeField] WorldObjectsDatabaseSO worldObjectsDatabase;
    public float yPlacementOffset = 0.25f;
    public GridLayout GridLayout;
    public Tilemap GroundTilemap;
    public Tilemap WaterTilemap;
    public Tilemap TempTilemap;
    public GridLayout WorldObjectsGrid;
    public Dictionary<int, GridLayout> WorldTilemapsDictionary;
    [HideInInspector] public Chunk currentChunk = null;

    public GameObject campfirePrefab;
    GameObject campfireInstance;
    Vector3 currentChunkCenter = Vector3.zero;
    public Vector3 gridOffset;

    [Space(10f)]
    [Header("Generation Settings")]
    [Space(5f)]
    public HeightMapSettings heightMapSettings;
    public ChunkManagementSettings chunkManagementSettings;
    public int chunkSize { get => chunkManagementSettings.chunkSize; }
    public int worldWidthInChunks { get => chunkManagementSettings.worldWidthInChunks; }
    public int worldHeightInChunks { get => chunkManagementSettings.worldHeightInChunks; }
    public Vector2Int sampleCenter;
    public int totalWidthTiles = 0;
    public int totalHeightTiles = 0;

    bool _worldGenerated = false;
    bool _justStartedGeneration = true;
    int _chunksMovedAmount = 0;
    Queue<Vector3Int> _chunkLoadQueue = new();
    Queue<Vector3Int> _chunkUnloadQueue = new();
    int _loadFrameCounter = 4;
    int _unloadFrameCounter = 4;
    Vector3Int _currentChunkPosition;
    Vector3Int _lastChunkPosition;
    Vector3Int _lastChunkDistanceCheckPosition;

    static Stopwatch generationStopwatch;

    public static Action OnWorldCreated;
#endregion

#region Unity Methods
#if UNITY_EDITOR
    void OnValidate() {
        totalWidthTiles = chunkManagementSettings.worldWidthInChunks * chunkManagementSettings.chunkSize;
        totalHeightTiles = chunkManagementSettings.worldHeightInChunks * chunkManagementSettings.chunkSize;
        chunkManagementSettings.ValidateValues();
    }
#endif

    void OnEnable() {
        CameraManager.OnChunkCrossed += UpdateCurrentChunk;
        OnWorldCreated += SpawnCampfire;
        OnWorldCreated += UpdateChunks;
    }

    void OnDisable() {
        CameraManager.OnChunkCrossed -= UpdateCurrentChunk;
        OnWorldCreated -= SpawnCampfire;
        OnWorldCreated -= UpdateChunks;
    }

    protected override void Awake() {
        base.Awake();

        RecordTilemaps();
        Random.InitState(WorldData.WorldSeed);
    }

    public int chunksToLoad = 0;
    public int chunksToUnload = 0;

    void Update() {
        if (WorldData.IsNull() || !_worldGenerated) return;

        if (_chunkLoadQueue != null) {
            chunksToLoad = _chunkLoadQueue.Count;
        }
        if (_chunkUnloadQueue != null) {
            chunksToUnload = _chunkUnloadQueue.Count;
        }

        ProcessChunkLoadingQueue();
        ProcessChunkUnloadingQueue();
    }

#endregion

#region Generation Methods
    void RecordTilemaps() {
        WorldTilemapsDictionary = new Dictionary<int, GridLayout> {
            { 0, WaterTilemap},
            { 1, GroundTilemap },
            { 2, TempTilemap }
        };
    }

    Thread chunkUpdatingThread;

    public void InitializeWorldGeneration(bool firstGeneration) {
        OnWorldGenerationStarted();
        GenerateWorld(firstGeneration);
    }

    void GenerateWorld(bool firstGeneration) {
        if (firstGeneration) {
            ChunkData = new();
            GridData = new();
        }
        else {
            ChunkData = WorldData.ChunkData;
            GridData = WorldData.GridData;
        }

        totalWidthTiles = worldWidthInChunks * chunkSize;
        totalHeightTiles = worldHeightInChunks * chunkSize;

        SetWorldOffset();
        HeightMap = HeightMapGenerator.GenerateHeightMap(totalWidthTiles, totalHeightTiles, heightMapSettings, sampleCenter);
        mapPreview.DrawMapInEditor(totalWidthTiles, totalHeightTiles, HeightMap, heightMapSettings);

        int halfWidthInChunks = worldWidthInChunks / 2;
        int halfHeightInChunks = worldHeightInChunks / 2;

        Chunk chunk = null;
        Vector3Int chunkLocalPos;
        Vector3Int chunkGridPos;

        Helpers.ClearConsole();

        for (int y = 0; y < worldHeightInChunks; y++) {
            for (int x = 0; x < worldWidthInChunks; x++) {
                chunkLocalPos = new Vector3Int(
                    -halfWidthInChunks + (worldWidthInChunks.IsEven() ? 1 : 0) + x,
                    -halfHeightInChunks + (worldHeightInChunks.IsEven() ? 1 : 0) + y, 0);
                chunkGridPos = new Vector3Int(x, y, 0);

                chunk = ChunkData.CreateChunk(chunk, chunkLocalPos, chunkGridPos, chunkSize, GroundTilemap, WaterTilemap);
                chunk = terrainGenerator.GenerateTerrainChunk(chunk, HeightMap, heightMapSettings);
                chunk = terrainGenerator.GenerateTrees(chunk, HeightMap, heightMapSettings, GridData, worldObjectsDatabase.GetObjectDataByID(1), GridLayout, yPlacementOffset);
                ChunkData.UpdateChunk(chunkLocalPos, false);
            }
        }

        currentChunk = ChunkData.GetChunkAt(Vector3Int.zero);
        _currentChunkPosition = Vector3Int.zero;
        _lastChunkPosition = Vector3Int.zero;
        _lastChunkDistanceCheckPosition = Vector3Int.zero;

        OnWorldGenerationFinished();
    }

    public void UpdateCurrentChunk() {
        _lastChunkDistanceCheckPosition = _currentChunkPosition;
        _currentChunkPosition = WorldDataHelper.ChunkIDFromChunkPosition(chunkSize, CameraManager.instance.CameraChunkPosition);

        if (WorldData.IsNull() || ChunkData.IsNull() || currentChunk.IsNull()) return;

        if (_currentChunkPosition != _lastChunkPosition) {
            if (_chunksMovedAmount >= chunkManagementSettings.chunksMovedThreshold  || _justStartedGeneration) {
                currentChunk = ChunkData.GetChunkAt(_currentChunkPosition);

                if (chunkUpdatingThread.IsNull() || !chunkUpdatingThread.IsAlive) {
                    Debug.Log("Started Chunk Updating Thread!");
                    chunkUpdatingThread = new Thread(UpdateChunks);
                    chunkUpdatingThread.Start();
                }

                _chunksMovedAmount = 0;
                _justStartedGeneration = false;
            }

            _lastChunkPosition = _currentChunkPosition;
            _chunksMovedAmount++;
        }
    }

    public async void UpdateChunks() {
        await LoadNearChunks();
        await UnloadFarChunks();
    }

    public async Task ReloadWorld() {
        StopWorldGeneration();
        await UnloadAllChunks();
        UpdateChunks();
    }

    async Task LoadNearChunks() {
        Vector3Int direction = _currentChunkPosition - _lastChunkDistanceCheckPosition;

        Direction loadDir = WorldDataHelper.GetDirection(direction);

        List<Vector3Int> positionsList = WorldDataHelper.GetNearPositions(_currentChunkPosition, chunkManagementSettings.chunkRenderDistance, loadDir);
        List<Vector3Int> newPositionsList = new List<Vector3Int>(positionsList.Where(p => ChunkData.CheckIfExists(p) && !ChunkData.CheckIfLoaded(p)));

        Vector3Int checkPos;

        lock (_chunkLoadQueue) {
            for (int i = 0; i < newPositionsList.Count; i++) {
                checkPos = newPositionsList.GetElement(i);

                _chunkLoadQueue.Enqueue(checkPos);
            }
        }
    }

    async Task UnloadFarChunks() {
        lock (_chunkUnloadQueue) {
            foreach (var chunk in ChunkData.ChunksDictionary) {
                Vector3Int distance = (_currentChunkPosition - chunk.Key).Abs();
                if (chunk.Value.isLoaded && (
                    distance.x >= chunkManagementSettings.chunkUnloadingDistance ||
                    distance.y >= chunkManagementSettings.chunkUnloadingDistance)) {
                    _chunkUnloadQueue.Enqueue(chunk.Key);
                }
            }
        }
    }

    async Task UnloadAllChunks() {
        foreach (var pair in ChunkData.ChunksDictionary) {
            if (ChunkData.CheckIfLoaded(pair.Key)) {
                _chunkUnloadQueue.Enqueue(pair.Key);
            }
        }
    }

    void ProcessChunkLoadingQueue() {
        if (_chunkLoadQueue.IsNull() || _chunkLoadQueue.IsEmpty()) return;

        if (_chunkLoadQueue.Count > 0) {
            Vector3Int chunkLocalPos;
            _loadFrameCounter++;

            if (_loadFrameCounter % chunkManagementSettings.loadInterval == 0) {
                int chunksToProcess = Mathf.Min(chunkManagementSettings.chunksLoadedPerFrame, _chunkLoadQueue.Count);

                for (int i = 0; i < chunksToProcess; i++) {
                    chunkLocalPos = _chunkLoadQueue.Dequeue();

                    if (!ChunkData.CheckIfLoaded(chunkLocalPos)) {
                        ChunkData.UpdateChunk(chunkLocalPos, true);
                    }
                }
            }
        }
    }

    void ProcessChunkUnloadingQueue() {
        if (_chunkUnloadQueue.IsNull() || _chunkUnloadQueue.IsEmpty()) return;

        if (_chunkUnloadQueue.Count > 0) {
            Vector3Int chunkLocalPos;
            _unloadFrameCounter++;

            if (_unloadFrameCounter % chunkManagementSettings.unloadInterval == 0) {
                int chunksToProcess = Mathf.Min(chunkManagementSettings.chunksUnloadedPerFrame, _chunkUnloadQueue.Count);

                for (int i = 0; i < chunksToProcess; i++) {
                    chunkLocalPos = _chunkUnloadQueue.Dequeue();
                    ChunkData.UpdateChunk(chunkLocalPos, false);
                }
            }
        }
    }

    void ClearWorldMap() {
        WaterTilemap.ClearAllTiles();
        GroundTilemap.ClearAllTiles();
        TempTilemap.ClearAllTiles();
    }

    void StopWorldGeneration() {
        if (_chunkLoadQueue.IsNotNull()) _chunkLoadQueue.Clear();
        _chunkLoadQueue = new();

        if (_chunkUnloadQueue.IsNotNull()) _chunkUnloadQueue.Clear();
        _chunkUnloadQueue = new();
    }

    public void SetWorldOffset(bool manualRandomSeed = false, int seed = 0) {
        int xOffset = 0;
        int yOffset = 0;

        if (WorldData.IsNotNull()) {
            Random.InitState(WorldData.WorldSeed);
            xOffset = Random.Range(-100000, 100000);
            yOffset = Random.Range(-100000, 100000);
        }

        heightMapSettings.noiseSettings.seed = WorldData.WorldSeed;
        heightMapSettings.noiseSettings.offset = new Vector2Int(xOffset, yOffset);
    }

    void OnWorldGenerationStarted() {
        _worldGenerated = false;

        generationStopwatch = Stopwatch.StartNew();

        ClearWorldMap();

        _chunkLoadQueue = new Queue<Vector3Int>();
        _chunkUnloadQueue = new Queue<Vector3Int>();

        _justStartedGeneration = true;
        _chunksMovedAmount = 0;
        Debug.Log("World Generation started...");
    }

    void OnWorldGenerationFinished() {
        generationStopwatch.Stop();
        _worldGenerated = true;
        WorldData.FirstGeneration = false;
        SaveData();
        OnWorldCreated?.Invoke();
        Debug.Log($"World Generation finished. Time taken: {generationStopwatch.ElapsedMilliseconds}ms");
    }

    public void SpawnCampfire() {
        currentChunkCenter = WorldObjectsGrid.CellToWorld(new Vector3Int(
            0 + chunkSize / 2,
            0 + chunkSize / 2,
            0)) + gridOffset;
        campfireInstance = Instantiate(campfirePrefab, currentChunkCenter, Quaternion.identity);
        campfireInstance.transform.SetParent(WorldObjectsGrid.transform);
    }

    public void PreviewWorld() {
        SetWorldOffset();

        HeightMap heightMap;

        if (HeightMap.values == null) {
            heightMap = HeightMapGenerator.GenerateHeightMap(totalWidthTiles, totalHeightTiles, heightMapSettings, sampleCenter);
            HeightMap = heightMap;
        }
        else {
            heightMap = HeightMap;
        }

        mapPreview.DrawMapInEditor(totalWidthTiles, totalHeightTiles, heightMap, heightMapSettings);
    }
#endregion

#region World Serialization
    public void CreateData() {
        StopWorldGeneration();

        WorldData = new WorldData(true, DateTime.Now, true);
        SetWorldOffset();
        HeightMap = HeightMapGenerator.GenerateHeightMap(totalWidthTiles, totalHeightTiles, heightMapSettings, sampleCenter);
        GridData = new();
        mapPreview.DrawMapInEditor(totalWidthTiles, totalHeightTiles, HeightMap, heightMapSettings);

        DataPersistenceManager.Instance.WriteData(WorldData);

        Debug.Log("Created World Data!");
    }

    public void SaveData() {
        if (WorldData.IsNull()) {
            Debug.Log("Failed to save, World Data is null!");
            return;
        }

        StopWorldGeneration();

        WorldData inputWorld = WorldData;
        inputWorld.CreationTime = WorldData.CreationTime;
        inputWorld.LastSavedTime = DateTime.Now;
        inputWorld.ChunkData.ModifiedChunksDictionary = WorldData.ChunkData.ModifiedChunksDictionary;
        inputWorld.GridData = WorldData.GridData;

        // int modifiedChunks = 0;

        // foreach (KeyValuePair<Vector3Int, Chunk> pair in WorldData.ChunkData.ChunksDictionary) {
        //     if (WorldData.ChunkData.CheckIfModified(pair.Value) && !inputWorld.ChunkData.ModifiedChunksDictionary.ContainsKey(pair.Key)) {
        //         inputWorld.ChunkData.ChunksDictionary.AddOrReplace(pair.Key, pair.Value);
                
        //         modifiedChunks++;
        //     }
        // }

        // if (inputWorld.ChunkData.ChunksDictionary.IsEmpty() || modifiedChunks == 0) {
        //     Debug.Log($"No Chunk was modified...");
        // }
        // else {
        //     Debug.Log($"Saved {modifiedChunks} modified Chunks!");
        // }

        DataPersistenceManager.Instance.WriteData(inputWorld);

        if (Application.isPlaying) {
            Task.Run(ReloadWorld);
        }
    }

    public void LoadData() {
        StopWorldGeneration();

        if (DataPersistenceManager.Instance.CheckData(WorldData)) {
            WorldData = DataPersistenceManager.Instance.ReadData(WorldData);
        }
        else {
            CreateData();
        }

        if (Application.isPlaying) InitializeWorldGeneration(WorldData.FirstGeneration);
    }

    public void DeleteData() {
        StopWorldGeneration();
        
        if (DataPersistenceManager.Instance.ClearData(WorldData)) {
            ClearWorldMap();
            HeightMap = new HeightMap();
            Debug.Log($"Deleted World Data!");
        }
        else {
            Debug.Log($"World Data doesnt exist!");
        }
    }
    #endregion
}