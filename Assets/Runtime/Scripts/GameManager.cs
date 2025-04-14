using UnityEngine;
using ExtensionMethods;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public AudioManager AudioManager { get; private set; }
    public OptionsManager OptionsManager { get; private set; }

    private void Awake() {
        if (Instance.IsNull()) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"Setting GameManager Instance to {this}");
            InitializeManagers();
        }
        else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    private void InitializeManagers() {
        AudioManager = GetComponentInChildren<AudioManager>();
        OptionsManager = GetComponentInChildren<OptionsManager>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ExitGame();
        }
        else if (Input.GetKeyDown(KeyCode.R)) {
            RestartLevel();
        }
        else if (Input.GetKeyDown(KeyCode.L)) {
            PauseEditor();
        }
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
