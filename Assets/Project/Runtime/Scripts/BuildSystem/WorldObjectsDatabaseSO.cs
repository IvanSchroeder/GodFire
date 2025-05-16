using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "WorldObjectsDatabase", menuName = "Data/Objects/World Objects Database")]
public class WorldObjectsDatabaseSO : ScriptableObject {
    public List<WorldObjectData> ObjectsDataList;

    public WorldObjectData GetObjectDataByID(int ID) {
        return ObjectsDataList.FirstOrDefault(wo => wo.ID == ID);
    }
}

[Serializable]
public class WorldObjectData {
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;
    [field: SerializeField] public GameObject Prefab { get; private set; }
}