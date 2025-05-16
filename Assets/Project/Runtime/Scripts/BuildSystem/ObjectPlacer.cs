using UnityEngine;
using System.Collections.Generic;
using System;
using UnityUtilities;

public class ObjectPlacer : Singleton<ObjectPlacer> {
    public List<GameObject> PlacedWorldObjectsList = new();

    public int PlaceObjectAt(GameObject prefab, Vector3 position, float yOffset) {
        GameObject worldObject = Instantiate(prefab);
        worldObject.transform.position = position.Add(y: yOffset);
        PlacedWorldObjectsList.Add(worldObject);

        return PlacedWorldObjectsList.Count - 1;
    }

    public void RemoveObjectAt(int gameObjectIndex) {
        if (PlacedWorldObjectsList.Count <= gameObjectIndex || PlacedWorldObjectsList[gameObjectIndex] == null)
            return;
        GameObjectExtensions.Destroy(PlacedWorldObjectsList.GetElement(gameObjectIndex));
        PlacedWorldObjectsList[gameObjectIndex] = null;
    }

    // public void SetObjectActiveStateAt(int gameObjectIndex) {

    // }
}
