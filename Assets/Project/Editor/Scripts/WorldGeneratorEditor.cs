using UnityEngine;
using UnityEditor;
using UnityUtilities;

[CustomEditor(typeof(WorldGenerator))]
public class WorldGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        WorldGenerator worldGenerator = (WorldGenerator)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Create and Generate World")) {
            worldGenerator.CreateAndGenerateNewWorld();
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Generate World")) {
            worldGenerator.GenerateWorld();
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Create World Data")) {
            worldGenerator.CreateNewWorldData();
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Load World Data")) {
            worldGenerator.LoadWorldData();
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Save World Data")) {
            worldGenerator.SaveWorldData();
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Delete World Data")) {
            worldGenerator.DeleteWorldData();
            EditorUtility.SetDirty(target);
        }
    }
}
