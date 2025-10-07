using UnityEngine;
using UnityUtilities;

public class GameManager : Singleton<GameManager> {
    public GameManager GameManagerInstance { get => Instance; }
    public AudioManager AudioManagerInstance { get => AudioManager.Instance; }
    public OptionsManager OptionsManagerInstance { get => OptionsManager.Instance; }
    public UIManager UIManagerInstance { get => UIManager.Instance; }

    public System.Random SystemRandom = new();

    protected override void Awake() {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }

    private async void Start() {
        await UIManagerInstance.Init();
    }

    private void Update() {
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


