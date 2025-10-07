using UnityEngine;

public class RemovingState : IBuildingState {
    int gameObjectIndex = -1;
    GridLayout grid;
    GridData gridData;

    public RemovingState(GridLayout _grid, GridData _gridData) {
        grid = _grid;
        gridData = _gridData;

        // previewSystem.StartShowingRemovePreview();
    }

    public void EndState() {
        // previewSystem.StopShowingPreview();
    }

    public void PerformAction(Vector3Int gridPosition, float yPlacementOffset) {
        GridData selectedData = null;

        if (!gridData.CanPlaceObjectAt(gridPosition, Vector2Int.one)) {
            selectedData = gridData;
        }

        if (selectedData == null) {
            // sound
        }
        else {
            gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
            if (gameObjectIndex == -1)
                return;

            // selectedData.RemoveObjectAt(gridPosition);
            // objectPlacer.RemoveObjectAt(gameObjectIndex);
        }
    }

    public void UpdateState(Vector3Int gridPosition) {

    }
}
