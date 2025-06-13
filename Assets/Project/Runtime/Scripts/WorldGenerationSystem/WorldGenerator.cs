using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityUtilities;
using System;
using DG.Tweening;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Linq;
using Random = UnityEngine.Random;
using SaveSystem;
using WorldSimulation;
using System.Threading;
using Cysharp.Threading.Tasks;

public class WorldGenerator : Singleton<WorldGenerator>, IDataHandler {
#region Variables
    [Header("References")]
    [Space(5f)]
    public TerrainGenerator terrainGenerator;
    public MapPreview mapPreview;
    [HideInInspector] public WorldData WorldData;
    public ChunkData ChunkData;
    public GridData GridData;
    [SerializeField] ObjectsDatabaseSO worldObjectsDatabase;
    public float yPlacementOffset = 0.25f;
    public GridLayout GridLayout;
    public Tilemap GroundTilemap;
    public Tilemap WaterTilemap;
    public Tilemap TempTilemap;
    public GridLayout WorldObjectsGrid;
    public Dictionary<int, GridLayout> WorldTilemapsDictionary;
    [HideInInspector] public Chunk currentChunk = null;
    public Transform worldObjectsParentTransform = null;

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

    public Vector3 limitN => GridLayout.CellToWorld(Vector3Int.zero.Add((totalWidthTiles + chunkManagementSettings.chunkSize) / 2, (totalHeightTiles + chunkManagementSettings.chunkSize) / 2)).Floor().Add(y: -0.25f);
    public Vector3 limitS => GridLayout.CellToWorld(Vector3Int.zero.Add(-(totalWidthTiles - chunkManagementSettings.chunkSize) / 2, -(totalHeightTiles - chunkManagementSettings.chunkSize) / 2)).Floor().Add(y: -0.25f);
    public Vector3 limitW => GridLayout.CellToWorld(Vector3Int.zero.Add(-(totalWidthTiles - chunkManagementSettings.chunkSize) / 2, (totalHeightTiles + chunkManagementSettings.chunkSize) / 2)).Floor().Add(y: -0.25f);
    public Vector3 limitE => GridLayout.CellToWorld(Vector3Int.zero.Add((totalWidthTiles + chunkManagementSettings.chunkSize) / 2, -(totalHeightTiles - chunkManagementSettings.chunkSize) / 2)).Floor().Add(y: -0.25f);

    static Stopwatch generationStopwatch;

    public static readonly int CAMPFIRE = 0;
    public static readonly int TREE = 1;

    [Range(0, 5)] public float objectSpawnSeconds = 0.001f;
    Thread chunkUpdatingThread;

    public static Action OnWorldCreated;
    public static Action OnWorldReloaded;
    public static Action OnWorldCleared;
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
        OnWorldCreated += UpdateChunks;
        OnWorldReloaded += UpdateChunks;
    }

    void OnDisable() {
        CameraManager.OnChunkCrossed -= UpdateCurrentChunk;
        OnWorldCreated -= UpdateChunks;
        OnWorldReloaded -= UpdateChunks;
    }

    protected override void Awake() {
        base.Awake();

        Init();
    }

    public void Init() {
        RecordTilemaps();
        Random.InitState(WorldData.WorldSeed);
    }

    void Update() {
        if (WorldData.IsNull() || !_worldGenerated) return;
        ProcessChunkLoadingQueue();
        ProcessChunkUnloadingQueue();
    }

#endregion

#region Generation Methods
    private void RecordTilemaps() {
        WorldTilemapsDictionary = new Dictionary<int, GridLayout> {
            { 0, WaterTilemap},
            { 1, GroundTilemap },
            { 2, TempTilemap }
        };
    }

    public async UniTask InitializeWorldGeneration(bool firstGeneration) {
        OnWorldGenerationStarted();
        
        await GenerateTerrain(firstGeneration);
        await SpawnWorldObjects(firstGeneration);

        OnWorldGenerationFinished();

        await UniTask.Yield();
    }

    private async UniTask GenerateTerrain(bool firstGeneration) {
        totalWidthTiles = worldWidthInChunks * chunkSize;
        totalHeightTiles = worldHeightInChunks * chunkSize;

        int halfWidthInChunks = worldWidthInChunks / 2;
        int halfHeightInChunks = worldHeightInChunks / 2;

        Chunk chunk = null;
        Vector3Int chunkLocalPos;
        Vector3Int chunkGridPos;

    #if UNITY_EDITOR
        Helpers.ClearConsole();
    #endif

        SetWorldOffset();

        if (firstGeneration) {
            ChunkData = new();
            ChunkData.terrainHeightMap = HeightMapGenerator.GenerateHeightMap(totalWidthTiles, totalHeightTiles, heightMapSettings, sampleCenter);
            ChunkData.terrainIDsMap = new int[totalWidthTiles,totalHeightTiles];
            ChunkData.treesMapValues = terrainGenerator.GenerateWorldTreesMap(ChunkData.terrainHeightMap.values, heightMapSettings);
        }
        else {
            ChunkData = WorldData.ChunkData;
        }

        for (int y = 0; y < worldHeightInChunks; y++) {
            for (int x = 0; x < worldWidthInChunks; x++) {
                chunkLocalPos = new Vector3Int(
                    -halfWidthInChunks + (worldWidthInChunks.IsEven() ? 1 : 0) + x,
                    -halfHeightInChunks + (worldHeightInChunks.IsEven() ? 1 : 0) + y, 0);
                chunkGridPos = new Vector3Int(x, y, 0);

                // Create the chunks. Later on add check for previously modified chunks because they are different from world generated chunks.
                chunk = ChunkData.CreateChunk(chunk, chunkLocalPos, chunkGridPos, chunkSize, GroundTilemap, WaterTilemap);
                chunk = terrainGenerator.GenerateTerrainChunk(chunk, ChunkData.terrainHeightMap, heightMapSettings);

                ChunkData.UpdateChunk(chunkLocalPos, false);

                await WaitFor.Delay(0.001f, true);
            }
        }

        currentChunk = ChunkData.GetChunkAt(Vector3Int.zero);
        _currentChunkPosition = Vector3Int.zero;
        _lastChunkPosition = Vector3Int.zero;
        _lastChunkDistanceCheckPosition = Vector3Int.zero;

        await UniTask.Yield();
    }

    private async UniTask SpawnWorldObjects(bool firstGeneration) {
        if (firstGeneration) {
            await CreateNewWorldObjects();
        }
        else {
            await PlaceSavedWorldObjects();
        }
    }

    private async UniTask CreateNewWorldObjects() {
        GridData = new();
        await SpawnCampfire();
        await SpawnTrees();
    }

    private async UniTask PlaceSavedWorldObjects() {
        GridData.WorldObjectsDictionary = new();
        ObjectData savedObjectData;

        foreach (var data in GridData.WorldObjectsDataList) {
            savedObjectData = worldObjectsDatabase.GetObjectDataByID(data.ObjectPlacementData.ObjectID);
            PlaceObjectAt(savedObjectData.Prefab, data.ObjectPlacementData.OriginalPosition, savedObjectData, data);

            await WaitFor.Delay(objectSpawnSeconds, true);
        }

        await UniTask.Yield();
    }
 
    private async UniTask SpawnCampfire() {
        Vector3Int gridPosition = _currentChunkPosition.Add(chunkSize / 2, chunkSize / 2);
        ObjectData objectData = worldObjectsDatabase.GetObjectDataByID(CAMPFIRE);

        PlaceObjectAt(objectData.Prefab, gridPosition, objectData, new CampfireData());
        
        await WaitFor.Delay(objectSpawnSeconds, true);

        await UniTask.Yield();
    }

    private async UniTask SpawnTrees() {
        Chunk chunk;
        ObjectData objectData = worldObjectsDatabase.GetObjectDataByID(TREE);

        for (int y = -chunkManagementSettings.chunkRenderDistance; y <= chunkManagementSettings.chunkRenderDistance; y++) {
            for (int x = -chunkManagementSettings.chunkRenderDistance; x <= chunkManagementSettings.chunkRenderDistance; x++) {
                chunk = ChunkData.GetChunkAt(_currentChunkPosition.Add(x, y));
                chunk = terrainGenerator.PlaceTrees(chunk, ChunkData.treesMapValues, objectData, GridData);

                await WaitFor.Delay(objectSpawnSeconds, true);
            }
        }

        await UniTask.Yield();
    }

    public void UpdateCurrentChunk() {
        _lastChunkDistanceCheckPosition = _currentChunkPosition;
        _currentChunkPosition = WorldDataHelper.ChunkIDFromChunkPosition(chunkSize, CameraManager.instance.CameraChunkPosition);

        if (WorldData.IsNull() || ChunkData.IsNull()) return;

        if (_currentChunkPosition != _lastChunkPosition) {
            if (_chunksMovedAmount >= chunkManagementSettings.chunksMovedThreshold  || _justStartedGeneration) {
                currentChunk = ChunkData.GetChunkAt(_currentChunkPosition);

                if (chunkUpdatingThread.IsNull() || !chunkUpdatingThread.IsAlive || currentChunk.IsNotNull()) {
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

    private async void UpdateChunks() {
        await UnloadFarChunks();
        await LoadNearChunks();
    }

    private async UniTask ReloadWorld() {
        await UnloadAllChunks();
        OnWorldReloaded?.Invoke();
    }

    private async UniTask LoadNearChunks() {
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

        await UniTask.Yield();
    }

    private async UniTask UnloadFarChunks() {
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

        await UniTask.Yield();
    }

    private async UniTask UnloadAllChunks() {
        StopWorldGeneration();

        foreach (var pair in ChunkData.ChunksDictionary) {
            if (ChunkData.CheckIfLoaded(pair.Key)) {
                pair.Value.Unload();
                // _chunkUnloadQueue.Enqueue(pair.Key);
            }
        }

        await UniTask.Yield();
    }

    private void ProcessChunkLoadingQueue() {
        if (_chunkLoadQueue.IsNullOrEmpty()) return;

        if (_chunkLoadQueue.Count > 0) {
            Vector3Int chunkLocalPos;
            _loadFrameCounter++;

            if (_loadFrameCounter % chunkManagementSettings.loadInterval == 0) {
                int chunksToProcess = Mathf.Min(chunkManagementSettings.chunksLoadedPerFrame, _chunkLoadQueue.Count);

                for (int i = 0; i < chunksToProcess; i++) {
                    chunkLocalPos = _chunkLoadQueue.Dequeue();
                    Chunk chunk = ChunkData.UpdateChunk(chunkLocalPos, true);
                    chunk.firstTimeLoading = false;
                }
            }
        }
    }

    private void ProcessChunkUnloadingQueue() {
        if (_chunkUnloadQueue.IsNullOrEmpty()) return;

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

    private void ClearWorldMap() {
        WaterTilemap.ClearAllTiles();
        GroundTilemap.ClearAllTiles();
        TempTilemap.ClearAllTiles();
    }

    private void StopWorldGeneration() {
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
            heightMapSettings.NoiseSettings.seed = WorldData.WorldSeed;
        }
        else {
            heightMapSettings.NoiseSettings.seed = seed;
        }
        
        heightMapSettings.NoiseSettings.offset = new Vector2Int(xOffset, yOffset);
    }

    private void OnWorldGenerationStarted() {
        _worldGenerated = false;

        generationStopwatch = Stopwatch.StartNew();

        ClearWorldMap();
        worldObjectsParentTransform.DestroyChildren();

        _chunkLoadQueue = new Queue<Vector3Int>();
        _chunkUnloadQueue = new Queue<Vector3Int>();

        _justStartedGeneration = true;
        _chunksMovedAmount = 0;
        Debug.Log("World Generation started...");
    }

    private void OnWorldGenerationFinished() {
        generationStopwatch.Stop();
        mapPreview.DrawMapInEditor(totalWidthTiles, totalHeightTiles, ChunkData.terrainHeightMap, heightMapSettings);
        _worldGenerated = true;
        WorldData.FirstGeneration = false;
        OnWorldCreated?.Invoke();
        Debug.Log($"World Generation finished. Time taken: {generationStopwatch.ElapsedMilliseconds}ms");
    }

    public void PreviewWorld() {
        SetWorldOffset();

        HeightMap heightMap;

        if (WorldData.IsNull() || WorldData.ChunkData.terrainHeightMap.values == null) {
            heightMap = HeightMapGenerator.GenerateHeightMap(totalWidthTiles, totalHeightTiles, heightMapSettings, sampleCenter);
        }
        else {
            heightMap = WorldData.ChunkData.terrainHeightMap;
        }

        mapPreview.DrawMapInEditor(totalWidthTiles, totalHeightTiles, heightMap, heightMapSettings);
    }

    public void PreviewWorldInEditor() {
        SetWorldOffset(true, 0);

        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(totalWidthTiles, totalHeightTiles, heightMapSettings, sampleCenter);

        mapPreview.DrawMapInEditor(totalWidthTiles, totalHeightTiles, heightMap, heightMapSettings);
    }

    public void ClearGameplayScene() {
        _worldGenerated = false;
        StopWorldGeneration();

        ClearWorldMap();
        ClearWorldObjects();

        OnWorldCleared?.Invoke();
    }
#endregion

#region World Objects Methods
    public void PlaceObjectAt(GameObject prefab, Vector3Int gridPosition, ObjectData objectData, WorldObjectData worldObjectData) {
        GameObject spawnedObject = Instantiate(prefab);
        Vector3 worldPosition = GridLayout.CellToWorld(gridPosition);
        spawnedObject.transform.position = worldPosition.Add(y: objectData.Size.y * yPlacementOffset);
        if (worldObjectsParentTransform.IsNotNull()) spawnedObject.transform.SetParent(worldObjectsParentTransform);

        WorldObject worldObject = spawnedObject.GetComponentInHierarchy<WorldObject>();

        GridData.AddObject(gridPosition, worldObject, objectData, worldObjectData);
    }

    public void RemoveObject(Vector3Int gridPosition) {
        if (GridData.WorldObjectsDictionary.IsNullOrEmpty() || !GridData.WorldObjectExists(gridPosition))
            return;

        WorldObject worldObject = GridData.WorldObjectsDictionary.GetValueOrDefault(gridPosition);

        if (worldObject.IsNull())
            return;

        GridData.RemoveObject(worldObject);
        worldObject.transform.gameObject.Destroy();
    }

    public void ClearWorldObjects() {
        GridData = new();
        worldObjectsParentTransform.DestroyChildren();
    }
#endregion

#region World Serialization
    public async UniTask CreateData() {
        StopWorldGeneration();

        WorldData = new WorldData(true, DateTime.Now, true);

        SetWorldOffset();
        ChunkData.terrainHeightMap = HeightMapGenerator.GenerateHeightMap(totalWidthTiles, totalHeightTiles, heightMapSettings, sampleCenter);
        ChunkData.terrainIDsMap = new int[totalWidthTiles,totalHeightTiles];
        ChunkData.treesMapValues = terrainGenerator.GenerateWorldTreesMap(ChunkData.terrainHeightMap.values, heightMapSettings);
        GridData.worldObjectsValues = HeightMapGenerator.GenerateEmptyMap(totalWidthTiles, totalHeightTiles, 0);

        WorldData.ChunkData = ChunkData;
        WorldData.GridData = GridData;

        PreviewWorld();

        if (!Application.isPlaying) await WriteWorldData();

        Debug.Log("Created World Data!");
    }

    public async UniTask SaveData() {
        if (WorldData.IsNull()) {
            Debug.Log("Failed to save, World Data is null!");
            return;
        }

        StopWorldGeneration();
        
        await WriteWorldData();

        if (Application.isPlaying) {
            await UniTask.RunOnThreadPool(ReloadWorld);
        }

        // WorldData inputWorld = WorldData;
        // inputWorld.CreationTime = WorldData.CreationTime;
        // inputWorld.LastSavedTime = DateTime.Now;
        // inputWorld.ChunkData.ModifiedChunksDictionary = WorldData.ChunkData.ModifiedChunksDictionary;
        // inputWorld.GridData = WorldData.GridData;

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
    }

    public async UniTask LoadData() {
        StopWorldGeneration();

        if (DataPersistenceManager.Instance.CheckData(WorldData)) {
            WorldData = DataPersistenceManager.Instance.ReadData(WorldData);
            ChunkData = WorldData.ChunkData;
            GridData = WorldData.GridData;
        }
        else {
            await CreateData();
        }

        PreviewWorld();

        if (Application.isPlaying) await InitializeWorldGeneration(WorldData.FirstGeneration);

        await UniTask.Yield();
    }

    public async UniTask DeleteData() {
        StopWorldGeneration();
        
        if (DataPersistenceManager.Instance.ClearData(WorldData)) {
            ClearGameplayScene();
            ChunkData = new();
            Debug.Log($"Deleted World Data!");
        }
        else {
            Debug.Log($"World Data doesnt exist!");
        }

        await UniTask.Yield();
    }

    private async UniTask WriteWorldData() {
        WorldData.LastSavedTime = DateTime.Now;
        WorldData.ChunkData = ChunkData;

        List<WorldObjectData> SavedWorldObjectsList = new();

        foreach (WorldObject worldObject in GridData.WorldObjectsDictionary.Values) {
            if (SavedWorldObjectsList.Contains(worldObject.worldObjectData)) continue;

            SavedWorldObjectsList.Add(worldObject.worldObjectData);

            await UniTask.Yield();
        }

        GridData.WorldObjectsDataList = new(SavedWorldObjectsList);
        WorldData.GridData = GridData;

        DataPersistenceManager.Instance.WriteData(WorldData);

        await UniTask.Yield();
    }
    #endregion


    public void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(limitN, limitN.Add(y: 5f));
        Gizmos.DrawLine(limitS, limitS.Add(y: 5f));
        Gizmos.DrawLine(limitW, limitW.Add(y: 5f));
        Gizmos.DrawLine(limitE, limitE.Add(y: 5f));

        Gizmos.DrawLine(limitN, limitE);
        Gizmos.DrawLine(limitE, limitS);
        Gizmos.DrawLine(limitS, limitW);
        Gizmos.DrawLine(limitW, limitN);
    }
}