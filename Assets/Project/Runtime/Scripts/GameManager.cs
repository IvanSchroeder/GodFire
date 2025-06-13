using UnityEngine;
using UnityUtilities;

public class GameManager : Singleton<GameManager> {
    public AudioManager AudioManager { get; private set; }
    public OptionsManager OptionsManager { get; private set; }

    public System.Random SystemRandom = new();

    protected override void Awake() {
        base.Awake();

        // DontDestroyOnLoad(gameObject);
        // InitializeManagers();
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


