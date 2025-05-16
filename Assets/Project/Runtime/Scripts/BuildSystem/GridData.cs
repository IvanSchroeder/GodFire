using UnityEngine;
using System.Collections.Generic;
using System;
using UnityUtilities;

[Serializable]
public class GridData {
    [SerializeField] public SerializedDictionary<Vector3Int, ObjectPlacementData> PlacedObjectsDictionary;

    public GridData() {
        PlacedObjectsDictionary = new();
    }

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int ID, int placedObjectIndex) {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        ObjectPlacementData data = new ObjectPlacementData(positionsToOccupy, ID, placedObjectIndex);

        foreach (Vector3Int position in positionsToOccupy) {
            if (PlacedObjectsDictionary.ContainsKey(position))
                throw new Exception($"Dictionary alread contais this cell position {position}");
            
            PlacedObjectsDictionary.AddIfNotExists(position, data);
        }
    }

    public void RemoveObjectAt(Vector3Int gridPosition) {
        foreach (Vector3Int position in PlacedObjectsDictionary.GetValueOrDefault(gridPosition).OccupiedPositionsList) {
            PlacedObjectsDictionary.Remove(position);
        }
    }

    List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize) {
        List<Vector3Int> returnValues = new();

        for (int y = 0; y < objectSize.y; y++) {
            for (int x = 0; x < objectSize.x; x++) {
                returnValues.Add(gridPosition + new Vector3Int(x, y, 0));
            }
        }

        return returnValues;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize) {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);

        foreach (var pos in positionsToOccupy) {
            if (PlacedObjectsDictionary.ContainsKey(pos))
                return false;
        }

        return true;
    }

    public int GetRepresentationIndex(Vector3Int gridPosition) {
        if (!PlacedObjectsDictionary.ContainsKey(gridPosition))
            return -1;
        
        return PlacedObjectsDictionary.GetValueOrDefault(gridPosition).PlacedObjectIndex;
    }
}
