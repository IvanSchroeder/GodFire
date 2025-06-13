using UnityEngine;
using System.Collections.Generic;
using System;
using UnityUtilities;
using System.Linq;
using Newtonsoft.Json;

[Serializable]
public class GridData {
    [JsonIgnore] public Dictionary<Vector3Int, WorldObject> WorldObjectsDictionary = new();
    [SerializeReference] public List<WorldObjectData> WorldObjectsDataList = new();
    [JsonIgnore] public HashSet<long> IDsHashSet = new();
    [JsonIgnore] public float[,] worldObjectsValues;

    public GridData() {}

    public void AddObject(Vector3Int gridPosition, WorldObject worldObject, ObjectData objectData, WorldObjectData worldObjectData) {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectData.Size);

        if (CanPlaceObjectAt(gridPosition, objectData.Size)) {
            foreach (Vector3Int position in positionsToOccupy) {
                WorldObjectsDictionary.AddIfNotExists(position, worldObject);
            }
            
            long objectUniqueID = GameManager.Instance.SystemRandom.NextLong();

            if (!IDsHashSet.Contains(objectUniqueID))
                IDsHashSet.Add(objectUniqueID);
            else {
                while (IDsHashSet.Contains(objectUniqueID)) {
                    objectUniqueID = GameManager.Instance.SystemRandom.NextLong();
                }
            }

            worldObjectData.ObjectPlacementData = new ObjectPlacementData(objectData.ID, gridPosition, objectData.Size, positionsToOccupy);
            worldObject.ID = objectUniqueID;

            // worldObject.worldObjectData.ObjectPlacementData = new ObjectPlacementData(objectData.ID, gridPosition, objectData.Size, positionsToOccupy);

            worldObject.SetObjectData(worldObjectData);

            IDsHashSet.Add(objectUniqueID);

            worldObject.Init();
        }
    }

    public void RemoveObject(WorldObject worldObject) {
        foreach (Vector3Int position in worldObject.worldObjectData.ObjectPlacementData.OccupiedPositionsList) {
            WorldObjectsDictionary.Remove(position);
        }

        IDsHashSet.Remove(worldObject.ID);
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
            if (WorldObjectExists(pos)) {
                return false;
                throw new Exception($"Dictionary already contains this cell position! : {pos}");
            }
        }

        return true;
    }

    public int GetRepresentationIndex(Vector3Int gridPosition) {
        if (!WorldObjectExists(gridPosition))
            return -1;
        
        // return PlacedObjectsDictionary.GetValueOrDefault(gridPosition).PlacedObjectIndex;
        return 1;
    }

    public bool WorldObjectExists(Vector3Int gridPosition) => WorldObjectsDictionary.ContainsKey(gridPosition);
}
