using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityUtilities;
using System.IO;
using System;
using DG.Tweening;
using System.Collections;
using System.Diagnostics;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;
using System.Linq;
using Newtonsoft.Json;
using Random = UnityEngine.Random;
using UnityEditor;

public class WorldGenerator : Singleton<WorldGenerator> {
    public TerrainGenerator terrainGenerator;
    public MapPreview mapPreview;
    public string dataPathFolderStructure = "/Data";
    public string dataPathSuffix = "/WorldData.json";
    string _dataPath = "";
    bool _isSavingData = false;

    public HeightMapSettings heightMapSettings;
    [Range(1, 100)] public int worldWidthInChunks;
    [Range(1, 100)] public int worldHeightInChunks;
    [Range(1, 128)] public int chunkSize = 32;
    public int totalWidthTiles = 0;
    public int totalHeightTiles = 0;
    public GridLayout GridLayout;
    public Tilemap GroundTilemap;
    public Tilemap WaterTilemap;
    public Tilemap PropsTilemap;
    public Tilemap TempTilemap;

    [SerializeField, Range(1, 30)] int chunkRenderDistance = 2;
    [SerializeField] int chunkUnloadingDistance;
    [SerializeField, Range(0, 10)] int chunkActiveThreshold = 1;
    [SerializeField, Range(1, 10)] int chunksMovedThreshold = 1;
    int chunksMovedAmount = 0;
    bool justStarted = true;
    Queue<Vector3Int> chunkLoadQueue = new Queue<Vector3Int>();
    [SerializeField, Range(1, 100)] int chunksLoadedPerFrame = 10;
    [SerializeField, Range(1, 100)] int loadInterval = 4;
    int loadFrameCounter = 4;
    Queue<Vector3Int> chunkUnloadQueue = new Queue<Vector3Int>();
    [SerializeField, Range(1, 100)] int chunksUnloadedPerFrame = 10;
    [SerializeField, Range(1, 100)] int unloadInterval = 4;
    int unloadFrameCounter = 4;

    public WorldData WorldData;
    public ChunkData ChunkData;
    public HeightMap HeightMap;
    public Dictionary<int, GridLayout> WorldTilemapsDictionary;
    public Chunk currentChunk = null;
    public Chunk lastChunk = null;
    Vector3Int _currentChunkPosition;
    Vector3Int _lastChunkPosition;
    Vector3Int _lastChunkDistanceCheckPosition;

    static Stopwatch generationStopwatch;

    public UnityEvent OnWorldCreated;

    [Range(1f, 10f)] public float chunksUpdateSeconds = 5f;
    CountdownTimer chunksUpdatingTimer;

    public float loadingSeconds = 0.1f;
    WaitForSeconds waitForSeconds;
    WaitForEndOfFrame waitForEndOfFrame;
    Coroutine worldGenerationCoroutine;
    Coroutine loadChunksCoroutine;
    Coroutine unloadChunksCoroutine;

    void OnEnable() {
        chunksUpdatingTimer.OnTimerStop += UpdateChunks;
    }

    void OnDisable() {
        chunksUpdatingTimer.OnTimerStop -= UpdateChunks;
    }

    void OnValidate() {
        totalWidthTiles = worldWidthInChunks * chunkSize;
        totalHeightTiles = worldHeightInChunks * chunkSize;
    }

    protected override void Awake() {
        base.Awake();

        chunksUpdatingTimer = new CountdownTimer(chunksUpdateSeconds);
    }

    void Start() {
        RecordTilemaps();
        waitForEndOfFrame = new WaitForEndOfFrame();

        LoadWorldData();
    }

    void Update() {
        chunkUnloadingDistance = chunkRenderDistance + chunkActiveThreshold;
        chunksUpdatingTimer.Tick(Time.deltaTime);

        ProcessChunkLoadingQueue();
        ProcessChunkUnloadingQueue();
    }

    void RecordTilemaps() {
        WorldTilemapsDictionary = new Dictionary<int, GridLayout> {
            { 0, WaterTilemap},
            { 1, GroundTilemap },
            { 2, PropsTilemap },
            { 3, TempTilemap }
        };
    }

#region Generation Methods
    public void GenerateWorld() {
        OnWorldGenerationStarted();

        waitForSeconds = new WaitForSeconds(loadingSeconds);
        chunksUpdatingTimer = new CountdownTimer(chunksUpdateSeconds);

        chunkLoadQueue = new Queue<Vector3Int>();
        chunkUnloadQueue = new Queue<Vector3Int>();

        if (worldGenerationCoroutine != null) {
            StopCoroutine(worldGenerationCoroutine);
            worldGenerationCoroutine = null;
        }

        worldGenerationCoroutine = StartCoroutine(GenerateWorldRoutine());
    }

    public void UpdateCurrentChunk() {
        _lastChunkDistanceCheckPosition = _currentChunkPosition;
        _currentChunkPosition = WorldDataHelper.ChunkIDFromChunkPosition(chunkSize, CameraManager.instance.CameraChunkPosition);

        if (ChunkData.IsNull() || currentChunk.IsNull()) return;

        if (_currentChunkPosition != _lastChunkPosition) {
            if (chunksMovedAmount >= chunksMovedThreshold  || justStarted) {
                lastChunk = currentChunk;
                currentChunk = ChunkData.GetChunkAt(_currentChunkPosition);
                UpdateChunks();
                chunksMovedAmount = 0;
                justStarted = false;
            }

            _lastChunkPosition = _currentChunkPosition;
            chunksMovedAmount++;
        }
    }

    public void UpdateChunks() {
        Debug.Log("Update Chunks");
        chunksUpdatingTimer?.Restart();
        LoadChunks();
        UnloadChunks();
    }

    void LoadChunks() {
        if (loadChunksCoroutine.IsNotNull()) {
            StopCoroutine(loadChunksCoroutine);
            loadChunksCoroutine = null;
        }

        loadChunksCoroutine = StartCoroutine(LoadNearChunksRoutine());
    }

    void UnloadChunks() {
        if (unloadChunksCoroutine.IsNotNull()) {
            StopCoroutine(unloadChunksCoroutine);
            unloadChunksCoroutine = null;
        }

        unloadChunksCoroutine = StartCoroutine(UnloadFarChunksRoutine());
    }

    public void ReloadWorld() {
        if (worldGenerationCoroutine.IsNotNull()) {
            StopCoroutine(worldGenerationCoroutine);
            worldGenerationCoroutine = null;
        }

        worldGenerationCoroutine = StartCoroutine(ReloadWorldRoutine());
    }

    void ProcessChunkLoadingQueue() {
        if (chunkLoadQueue.IsNull()) return;

        // Chunk chunk = null;
        Vector3Int chunkLocalPos;
        loadFrameCounter++;

        if (loadFrameCounter % loadInterval == 0) {
            for (int i = 0; i < chunksLoadedPerFrame && chunkLoadQueue.Count > 0; i++) {
                chunkLocalPos = chunkLoadQueue.Dequeue();

                if (!ChunkData.CheckIfLoaded(chunkLocalPos)) {
                    ChunkData.UpdateChunk(chunkLocalPos, true);
                }
            }
        }
    }

    void ProcessChunkUnloadingQueue() {
        if (chunkUnloadQueue.IsNull()) return;

        if (chunkUnloadQueue.Count > 0) {
            Vector3Int checkPos;
            unloadFrameCounter++;

            if (unloadFrameCounter % unloadInterval == 0) {
                int chunksToProcess = Mathf.Min(chunksUnloadedPerFrame, chunkUnloadQueue.Count);

                for (int i = 0; i < chunksToProcess; i++) {
                    checkPos = chunkUnloadQueue.Dequeue();
                    ChunkData.UpdateChunk(checkPos, false);
                }
            }
        }
    }

    void OnWorldGenerationStarted() {
        WaterTilemap.ClearAllTiles();
        GroundTilemap.ClearAllTiles();
        TempTilemap.ClearAllTiles();

        generationStopwatch = Stopwatch.StartNew();

        justStarted = true;
        chunksMovedAmount = 0;
        Debug.Log("World Generation started...");
    }

    void OnWorldGenerationFinished() {
        generationStopwatch.Stop();
        OnWorldCreated?.Invoke();
        Debug.Log($"World Generation finished. Time taken: {generationStopwatch.ElapsedMilliseconds} ms");
    }
#endregion

#region Generation Routines
    IEnumerator GenerateWorldRoutine() {
        ChunkData = WorldData.ChunkData;

        totalWidthTiles = worldWidthInChunks * chunkSize;
        totalHeightTiles = worldHeightInChunks * chunkSize;

        int xOffset = 0;
        int yOffset = 0;

        if (WorldData.IsNotNull()) {
            Random.InitState(WorldData.WorldSeed);
            xOffset = Random.Range(-100000, 100000);
            yOffset = Random.Range(-100000, 100000);
        }

        heightMapSettings.noiseSettings.offset = new Vector2Int(xOffset, yOffset);

        if (!WorldData.GeneratedHeightMap) {
            WorldData.HeightMap = HeightMapGenerator.GenerateHeightMap(totalWidthTiles, totalHeightTiles, heightMapSettings, Vector2.zero);
            WorldData.GeneratedHeightMap = true;
        }

        HeightMap = WorldData.HeightMap;
        Color[] colorMap = HeightMapGenerator.GenerateColorMap(totalWidthTiles, totalHeightTiles, HeightMap.values, heightMapSettings);
        mapPreview.DrawMapInEditor(HeightMap, totalWidthTiles, totalHeightTiles, colorMap);


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
                ChunkData.UpdateChunk(chunkLocalPos, false);
            }
        }

        // int chunksToSpawn = worldWidthInChunks * worldHeightInChunks;
        // int sequence = chunksToSpawn;
        // const int x = 0;
        // const int y = 1;
        // int[,] matrix = new int [2, sequence];
        // int dirX, dirY, prevX, prevY, curr;
        // dirX = dirY = prevX = prevY = default;

        // for (curr = 0; curr < sequence; curr++) {
        //     if (curr > 0) {
        //         prevX = matrix[x, curr - 1];
        //         prevY = matrix[y, curr - 1];
        //     }

        //     if (Mathf.Abs(prevX) == Mathf.Abs(prevY) && curr > 0) {
        //         dirX = dirY = 0;

        //         if (prevY > 0 && prevX > 0) dirX = -1;
        //         else if (prevY > 0 && prevX < 0) dirY = -1;
        //         else if (prevY < 0 && prevX < 0) dirX = 1;
        //         else if (prevY < 0 && prevX > 0) dirX = 1;
        //         else if (prevY == 0 && prevX == 0) dirX = 1;
        //     }
        //     else if (prevY < 0 && prevX > 0 && (Mathf.Abs(matrix[x, curr - 2]) == Mathf.Abs(matrix[y, curr -2]))) {
        //         dirX = 0;
        //         dirY = 1;
        //     }
        //     else if (prevX == 1 && prevY == 0) {
        //         dirX = 0;
        //         dirY = 1;
        //     }

        //     matrix[x, curr] = prevX + dirX;
        //     matrix[y, curr] = prevY + dirY;

        //     chunkLocalPos = new Vector3Int(matrix[x, curr], matrix[y, curr], 0);

        //     chunk = ChunkData.CreateChunk(chunk, chunkLocalPos, chunkSize, GroundTilemap, WaterTilemap, false);
        //     chunk = terrainGenerator.GenerateTerrainChunk(chunk, heightMap, heightMapSettings);
        // }

        currentChunk = ChunkData.GetChunkAt(Vector3Int.zero);
        lastChunk = ChunkData.GetChunkAt(Vector3Int.zero);
        _currentChunkPosition = Vector3Int.zero;
        _lastChunkPosition = Vector3Int.zero;
        _lastChunkDistanceCheckPosition = Vector3Int.zero;

        if (unloadChunksCoroutine.IsNotNull()) {
            StopCoroutine(unloadChunksCoroutine);
            unloadChunksCoroutine = null;
        }

        yield return unloadChunksCoroutine = StartCoroutine(UnloadAllChunksRoutine());

        if (loadChunksCoroutine.IsNotNull()) {
            StopCoroutine(loadChunksCoroutine);
            loadChunksCoroutine = null;
        }

        yield return loadChunksCoroutine = StartCoroutine(LoadNearChunksRoutine());

        OnWorldGenerationFinished();
        yield return null;
    }

    IEnumerator LoadNearChunksRoutine() {
        Vector3Int direction = _currentChunkPosition - _lastChunkDistanceCheckPosition;

        Direction loadDir = WorldDataHelper.GetDirection(direction);

        List<Vector3Int> positionsList = WorldDataHelper.GetNearPositions(_currentChunkPosition, chunkRenderDistance, loadDir);
        List<Vector3Int> newPositionsList = new List<Vector3Int>();

        // newPositionsList = new List<Vector3Int>(positionsList.Where(p => !ChunkData.CheckIfExists(p) || !ChunkData.CheckIfLoaded(p)));
        newPositionsList = new List<Vector3Int>(positionsList.Where(p => ChunkData.CheckIfExists(p) && !ChunkData.CheckIfLoaded(p)));

        Vector3Int checkPos;

        for (int i = 0; i < newPositionsList.Count; i++) {
            checkPos = newPositionsList.GetElement(i);

            chunkLoadQueue.Enqueue(checkPos);
        }

        chunksUpdatingTimer?.Restart();

        yield return null;
    }

    IEnumerator UnloadFarChunksRoutine() {
        // List<Chunk> farChunksList = ChunkData.LoadedChunksDictionary.Values.Where(c => c.isLoaded &&
        //     (Mathf.Abs(_currentChunkPosition.x - c.ID.x) >= chunkUnloadingDistance ||
        //     Mathf.Abs(_currentChunkPosition.y - c.ID.y) >= chunkUnloadingDistance)).ToList();

        foreach (var chunk in ChunkData.ChunksDictionary) {
            if (chunk.Value.isLoaded && (Mathf.Abs(_currentChunkPosition.x - chunk.Key.x) >= chunkUnloadingDistance || Mathf.Abs(_currentChunkPosition.y - chunk.Key.y) >= chunkUnloadingDistance)) {
                chunkUnloadQueue.Enqueue(chunk.Key);
            }
        }

        // Chunk chunk = null;

        // for (int i = 0; i < farChunksList.Count; i++) {
        //     chunkUnloadQueue.Enqueue(farChunksList.GetElement(i).ID);
        //     // chunk = ChunkData.UnloadChunk(farChunksList.GetElement(i));
        // }

        yield return null;
    }

    IEnumerator ReloadWorldRoutine() {
        if (unloadChunksCoroutine.IsNotNull()) {
            StopCoroutine(unloadChunksCoroutine);
            unloadChunksCoroutine = null;
        }

        yield return unloadChunksCoroutine = StartCoroutine(UnloadAllChunksRoutine());

        if (loadChunksCoroutine.IsNotNull()) {
            StopCoroutine(loadChunksCoroutine);
            loadChunksCoroutine = null;
        }

        yield return loadChunksCoroutine = StartCoroutine(LoadNearChunksRoutine());

        yield return null;
    }

    public IEnumerator UnloadAllChunksRoutine() {
        foreach (var chunk in ChunkData.ChunksDictionary) {
            if (chunk.Value.isLoaded) {
                chunkUnloadQueue.Enqueue(chunk.Key);
            }
        }

        yield return null;
    }
#endregion

#region World Serialization
    public void CreateAndGenerateNewWorld() {
        WaterTilemap.ClearAllTiles();
        GroundTilemap.ClearAllTiles();
        TempTilemap.ClearAllTiles();
        DeleteWorldData();
        LoadWorldData();
    }

    public void SaveWorldData() {
        StopAllCoroutines();
        if (chunkLoadQueue.IsNotNull()) chunkLoadQueue.Clear();
        chunkLoadQueue = null;
        if (chunkUnloadQueue.IsNotNull()) chunkUnloadQueue.Clear();
        chunkUnloadQueue = null;

        if (WorldData.IsNull()) {
            Debug.Log("Failed to save, World Data is null!");
            return;
        }

        _dataPath = SetDataPath(Application.dataPath, dataPathFolderStructure, dataPathSuffix);

        WorldData inputWorld = WorldData;
        inputWorld.HeightMap = HeightMap;
        inputWorld.SetSavedTime(DateTime.Now);
        inputWorld.ChunkData.ChunksDictionary = new Dictionary<Vector3Int, Chunk>();

        foreach (KeyValuePair<Vector3Int, Chunk> keyValuePair in WorldData.ChunkData.ChunksDictionary) {
            if (ChunkData.CheckIfModified(keyValuePair.Value)) {
                inputWorld.ChunkData.ChunksDictionary.AddIfNotExists(keyValuePair.Key, keyValuePair.Value);
            }
        }

        if (inputWorld.ChunkData.ChunksDictionary.IsEmpty()) {
            Debug.Log($"No Chunk was modified...");
        }

        string json = JsonConvert.SerializeObject(inputWorld, Formatting.Indented, new JsonSerializerSettings {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
        File.WriteAllText(_dataPath, json);
        AssetDatabase.Refresh();

        Debug.Log($"Saved World Data at {_dataPath}!");
    }

    public void LoadWorldData() {
        AssetDatabase.Refresh();
        string json = "";
        _dataPath = SetDataPath(Application.dataPath, dataPathFolderStructure, dataPathSuffix);

        bool worldExists = File.Exists(_dataPath);

        if (worldExists) {
            json = File.ReadAllText(_dataPath);
            WorldData = JsonConvert.DeserializeObject<WorldData>(json);
            Debug.Log($"Loaded World Data at {_dataPath}!");
        }
        else {
            CreateNewWorldData();
        }

        GenerateWorld();
    }

    public void CreateNewWorldData() {
        DeleteWorldData();

        WorldData = new WorldData(true, DateTime.Now);
        Debug.Log($"Created new World Data at {_dataPath}!");
        mapPreview.DrawMapInEditor(new HeightMap(), totalWidthTiles, totalHeightTiles, new Color[totalWidthTiles * totalHeightTiles]);

        SaveWorldData();

        // Add this world to list of created worlds
    }

    public void DeleteWorldData() {
        StopAllCoroutines();
        if (chunkLoadQueue.IsNotNull()) chunkLoadQueue.Clear();
        chunkLoadQueue = null;
        if (chunkUnloadQueue.IsNotNull()) chunkUnloadQueue.Clear();
        chunkUnloadQueue = null;

        _dataPath = SetDataPath(Application.dataPath, dataPathFolderStructure, dataPathSuffix);

        if (File.Exists(_dataPath)) {
            File.Delete(_dataPath);
            Debug.Log($"Deleted World Data at {_dataPath}!");
            WaterTilemap.ClearAllTiles();
            GroundTilemap.ClearAllTiles();
            TempTilemap.ClearAllTiles();

            mapPreview.DrawMapInEditor(new HeightMap(), totalWidthTiles, totalHeightTiles, new Color[totalWidthTiles * totalHeightTiles]);
        }
        else {
            Debug.Log($"World Data doesnt exist!");
            return;
        }

        AssetDatabase.Refresh();
    }

    public string SetDataPath(string _targetDataPath, string _dataPathFolderStructure, string _dataPathSuffix) {
        string path = string.Concat(_targetDataPath, _dataPathFolderStructure, _dataPathSuffix);
        return path;
    }
#endregion

    // private static void TraverseSpiralExpand<T>(T[,] matrix, Action<T> action) {
    //     var length = matrix.GetLength(0);
    //     if (length != matrix.GetLength(1)) {
    //         throw new InvalidDataException("SpiralExpand only supported on Square Matrix.");
    //     }

    //     // starting point
    //     var x = (length - 1) / 2;
    //     var y = (length - 1) / 2;

    //     // 0=>x++, 1=>y++, 2=>x--, 3=>y--
    //     var direction = 0;

    //     action(matrix[x, y]);
    //     for (var chainSize = 1; chainSize < length; chainSize++) {
    //         for (var j = 0; j < (chainSize < length - 1 ? 2 : 3); j++) {
    //             for (var i = 0; i < chainSize; i++) {
    //                 switch (direction) {
    //                     case 0:
    //                         x++;
    //                         break;
    //                     case 1:
    //                         y++;
    //                         break;
    //                     case 2:
    //                         x--;
    //                         break;
    //                     case 3:
    //                         y--;
    //                         break;
    //                 }
    //                 action(matrix[x, y]);
    //             }
    //             direction = (direction + 1) % 4;
    //         }
    //     }
    // }

    // private static Point[] TraverseSpiral(int width, int height) {
    //     int numElements = width * height + 1;
    //     Point[] points = new Point[numElements];

    //     int x = 0;
    //     int y = 0;
    //     int dx = 1;
    //     int dy = 0;
    //     int xLimit = width - 0;
    //     int yLimit = height - 1;
    //     int counter = 0;

    //     int currentLength = 1;
    //     while (counter < numElements) {
    //         points[counter] = new Point(x, y);

    //         x += dx;
    //         y += dy;

    //         currentLength++;
    //         if (dx > 0) {
    //             if (currentLength >= xLimit) {
    //                 dx = 0;
    //                 dy = 1;
    //                 xLimit--;
    //                 currentLength = 0;
    //             }
    //         } else if (dy > 0) {
    //             if (currentLength >= yLimit) {
    //                 dx = -1;
    //                 dy = 0;
    //                 yLimit--;
    //                 currentLength = 0;
    //             }
    //         } else if (dx < 0) {
    //             if (currentLength >= xLimit) {
    //                 dx = 0;
    //                 dy = -1;
    //                 xLimit--;
    //                 currentLength = 0;
    //             }
    //         } else if (dy < 0) {
    //             if (currentLength >= yLimit) {
    //                 dx = 1;
    //                 dy = 0;
    //                 yLimit--;
    //                 currentLength = 0;
    //             }
    //         }

    //         counter++;
    //     }

    //     Array.Reverse(points);
    //     return points;
    // }
}