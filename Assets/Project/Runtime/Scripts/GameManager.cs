using UnityEngine;
using UnityUtilities;

public class GameManager : Singleton<GameManager> {
    public AudioManager AudioManager { get; private set; }
    public OptionsManager OptionsManager { get; private set; }

    Camera mainCamera;
    public GameObject campfirePrefab;
    GameObject campfire;
    Vector3 currentChunkCenter = Vector3.zero;

    public Vector3 gridOffset;

    protected override void Awake() {
        base.Awake();

        // DontDestroyOnLoad(gameObject);
        // InitializeManagers();
    }

    void Start() {
        mainCamera = this.GetMainCamera();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ExitGame();
        }
        // else if (Input.GetKeyDown(KeyCode.R)) {
        //     RestartLevel();
        // }
        // else if (Input.GetKeyDown(KeyCode.L)) {
        //     PauseEditor();
        // }
    }

    private void InitializeManagers() {
        AudioManager = GetComponentInChildren<AudioManager>();
        OptionsManager = GetComponentInChildren<OptionsManager>();
    }

    public void SpawnCampfire() {
        if (campfire != null) return;

        currentChunkCenter = WorldGenerator.Instance.GroundTilemap.CellToWorld(new Vector3Int(
            0 + WorldGenerator.Instance.chunkSize / 2,
            0 + WorldGenerator.Instance.chunkSize / 2,
            0)) + gridOffset;
        campfire = Instantiate(campfirePrefab, currentChunkCenter, Quaternion.identity);
    }

    public void SetCurrentChunkCoordinates() {
        currentChunkCenter = WorldGenerator.Instance.GroundTilemap.CellToWorld(new Vector3Int(
            CameraManager.Instance.CameraChunkPosition.x + (WorldGenerator.Instance.chunkSize / 2),
            CameraManager.Instance.CameraChunkPosition.y + (WorldGenerator.Instance.chunkSize / 2),
            0)) + gridOffset;
    }

    public void ExitGame() {
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void RestartLevel() {
        this.RestartScene();
    }

    public void PauseEditor() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = UnityEditor.EditorApplication.isPaused.Toggle();
        #endif
    }
}


