using UnityEngine;
using UnityEditor;
using UnityUtilities;

[CustomEditor(typeof(WorldGenerator))]
public class WorldGeneratorEditor : Editor {
    WorldGenerator worldGenerator;

    public override void OnInspectorGUI() {
        worldGenerator = (WorldGenerator)target;

        if (DrawDefaultInspector()) {
            if (worldGenerator.mapPreview.autoUpdate) {
                worldGenerator.PreviewWorldInEditor();
            }
        }

        if (GUILayout.Button("Preview World")) {
            worldGenerator.PreviewWorld();
        }

        if (GUILayout.Button("Editor Preview World")) {
            worldGenerator.PreviewWorldInEditor();
        }

        if (GUILayout.Button("Create World Data")) {
            worldGenerator.CreateData();
        }

        if (GUILayout.Button("Load World Data")) {
            worldGenerator.LoadData();
        }

        if (GUILayout.Button("Save World Data")) {
            worldGenerator.SaveData();
        }

        if (GUILayout.Button("Delete World Data")) {
            worldGenerator.DeleteData();
        }

        EditorUtility.SetDirty(target);
    }

    // public void PreviewWorld() {
    //     worldGenerator.SetWorldOffset();

    //     HeightMap heightMap;

    //     if (worldGenerator.HeightMap.values == null) {
    //         heightMap = HeightMapGenerator.GenerateHeightMap(worldGenerator.totalWidthTiles, worldGenerator.totalHeightTiles, worldGenerator.heightMapSettings, worldGenerator.sampleCenter);
    //         worldGenerator.HeightMap = heightMap;
    //     }
    //     else {
    //         heightMap = worldGenerator.HeightMap;
    //     }

    //     worldGenerator.mapPreview.DrawMapInEditor(worldGenerator.totalWidthTiles, worldGenerator.totalHeightTiles, heightMap, worldGenerator.heightMapSettings);
    // }
}
