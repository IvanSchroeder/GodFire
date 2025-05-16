using UnityEngine;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

[Serializable]
public class ObjectPlacementData {
    [SerializeField] List<Vector3Int> _occupiedPositionsList;
    [SerializeField] int _ID;
    [SerializeField] int _placedObjectIndex;

    [JsonIgnore] public List<Vector3Int> OccupiedPositionsList { get => _occupiedPositionsList; set => _occupiedPositionsList = value; }
    [JsonIgnore] public int ID { get => _ID; set => _ID = value; }
    [JsonIgnore] public int PlacedObjectIndex { get => _placedObjectIndex; set => _placedObjectIndex = value; }

    public ObjectPlacementData(List<Vector3Int> _occupiedPositions, int _id, int _objectIndex) {
        _occupiedPositionsList = _occupiedPositions;
        _ID = _id;
        _placedObjectIndex = _objectIndex;
    }
}
