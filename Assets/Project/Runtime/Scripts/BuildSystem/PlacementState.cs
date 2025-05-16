using UnityEngine;
using UnityUtilities;

public interface IBuildingState {
    void EndState();
    void PerformAction(Vector3Int gridPosition, float yPlacementOffset);
    void UpdateState(Vector3Int gridPosition);
}

public class PlacementState : IBuildingState {
    int selectedObjectIndex = -1;
    int ID;
    GridLayout grid;
    WorldObjectsDatabaseSO worldObjectsDatabase;
    GridData GridData;
    ObjectPlacer objectPlacer;

    public PlacementState(int _ID, GridLayout _grid, WorldObjectsDatabaseSO _database, GridData _gridData, ObjectPlacer _objectPlacer) {
        ID = _ID;
        grid = _grid;
        worldObjectsDatabase = _database;
        GridData = _gridData;
        objectPlacer = _objectPlacer;

        selectedObjectIndex = worldObjectsDatabase.ObjectsDataList.FindIndex(ob => ob.ID == ID);
        if (selectedObjectIndex > -1) {

        }
        else {
            throw new System.Exception($"No object with ID {ID}");
        }
    }

    public void EndState() {

    }

    public void PerformAction(Vector3Int gridPosition, float yPlacementOffset) {
        if (!CheckPlacementValidity(gridPosition, selectedObjectIndex))
            return;

        WorldObjectData selectedWorldObject = worldObjectsDatabase.ObjectsDataList.GetElement(selectedObjectIndex);
        int index = objectPlacer.PlaceObjectAt(selectedWorldObject.Prefab, grid.CellToWorld(gridPosition), selectedWorldObject.Size.y * yPlacementOffset);
        GridData.AddObjectAt(gridPosition, selectedWorldObject.Size, selectedWorldObject.ID, index);
    }

    bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex) {
        return GridData.CanPlaceObjectAt(gridPosition, worldObjectsDatabase.ObjectsDataList.GetElement(selectedObjectIndex).Size);
    }

    public void UpdateState(Vector3Int gridPosition) {

    }
}