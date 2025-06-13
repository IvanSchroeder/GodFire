using UnityEngine;
using UnityEditor;
using SaveSystem;

[CustomEditor(typeof(DataPersistenceManager))]
public class DataPersistenceManagerEditor : Editor {
    DataPersistenceManager dataPersistence;

    public override void OnInspectorGUI() {
        dataPersistence = (DataPersistenceManager)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Create Profile")) {
            dataPersistence.CreateDirectory(DataPersistenceManager.SAVES_FOLDER + $"/{dataPersistence.selectedProfileId}");
        }

        if (GUILayout.Button("Delete Profile")) {
            dataPersistence.DeleteDirectory(DataPersistenceManager.SAVES_FOLDER + $"/{dataPersistence.selectedProfileId}");
        }

        EditorGUILayout.Space();

        // if (GUILayout.Button("Create Game Data")) {
        //     dataPersistence.CreateGameData();
        // }

        // if (GUILayout.Button("Save Game Data")) {
        //     dataPersistence.SaveGameData();
        // }

        // if (GUILayout.Button("Load Game Data")) {
        //     dataPersistence.LoadGameData();
        // }

        // if (GUILayout.Button("Delete Game Data")) {
        //     dataPersistence.DeleteGameData();
        // }

        EditorUtility.SetDirty(target);
    }
}