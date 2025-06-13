using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "WorldObjectsDatabase", menuName = "Data/Objects/World Objects Database")]
public class ObjectsDatabaseSO : ScriptableObject {
    public List<ObjectData> ObjectsDataList;

    public ObjectData GetObjectDataByID(int ID) {
        return ObjectsDataList.FirstOrDefault(wo => wo.ID == ID);
    }
}