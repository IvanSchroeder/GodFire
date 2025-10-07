using UnityEngine;
using System;
using UnityUtilities;
using UnityEngine.EventSystems;

public class InputManager : Singleton<InputManager> {
    [SerializeField] Camera mainCamera;
    Vector3 lastPosition;

    public static event Action OnClicked;
    public static event Action OnExit;

    void Start() {
        if (mainCamera.IsNull()) mainCamera = this.GetMainCamera();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            OnClicked?.Invoke();
        }

        if (Input.GetMouseButtonDown(1)) {
            OnExit?.Invoke();
        }
    }

    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetMouseScreenPosition() {
        return Input.mousePosition;
    }

    public Vector3 GetMouseWorldPosition() {
        return mainCamera.ScreenToWorld(GetMouseScreenPosition()).With(z: 0);
    }

    public Vector3Int GetMouseGridPosition(GridLayout gridLayout) {
        return gridLayout.WorldToCell(GetMouseWorldPosition());
    }
}
