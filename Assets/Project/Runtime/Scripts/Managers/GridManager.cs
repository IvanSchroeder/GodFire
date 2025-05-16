using UnityEngine;
using UnityUtilities;
using System.Linq;
using System.Collections.Generic;

public class GridManager : Singleton<GridManager> {
    [SerializeField] GridLayout grid;
    InputManager inputManager => InputManager.Instance;

    GridData gridData;
    [SerializeField] WorldObjectsDatabaseSO worldObjectsDatabase;
    public float yPlacementOffset = 0.25f;
    Vector3Int lastDetectedPosition = Vector3Int.zero;

    IBuildingState buildingState;

    void Start() {
        StopPlacement();
        gridData = new();
    }

    void Update() {
        if (buildingState == null) return;

        Vector3Int gridPosition = inputManager.GetMouseGridPosition(grid);

        if (lastDetectedPosition != gridPosition) {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
            // Debug.Log($"Updated detected Position to {lastDetectedPosition}");
        }
    }

    public void StartPlacement(int ID) {
        StopPlacement();

        buildingState = new PlacementState(ID, grid, worldObjectsDatabase, gridData, ObjectPlacer.Instance);

        InputManager.OnClicked += BuildWorldObject;
        InputManager.OnExit += StopPlacement;
    }

    public void StartRemoving() {
        StopPlacement();
        
        buildingState = new RemovingState(grid, gridData, ObjectPlacer.Instance);
        InputManager.OnClicked += BuildWorldObject;
        InputManager.OnExit += StopPlacement;
    }

    public void StopPlacement() {
        if (buildingState == null) return;

        buildingState.EndState();

        InputManager.OnClicked -= BuildWorldObject;
        InputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;

        buildingState = null;
    }

    void BuildWorldObject() {
        if (inputManager.IsPointerOverUI()) {
            return;
        }
        
        PlaceWorldObject();
    }

    void PlaceWorldObject() {
        Vector3Int gridPosition = inputManager.GetMouseGridPosition(grid);

        buildingState.PerformAction(gridPosition, yPlacementOffset);
    }
}