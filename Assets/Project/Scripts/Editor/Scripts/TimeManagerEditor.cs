using UnityEngine;
using UnityEditor;
using WorldSimulation;

[CustomEditor(typeof(TimeManager))]
public class TimeManagerEditor : Editor {
    TimeManager timeManager;

    public override void OnInspectorGUI() {
        timeManager = (TimeManager)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Create Time Data")) {
            timeManager.CreateData();
        }

        if (GUILayout.Button("Save Time Data")) {
            timeManager.SaveData();
        }

        if (GUILayout.Button("Load Time Data")) {
            timeManager.LoadData();
        }

        if (GUILayout.Button("Delete Time Data")) {
            timeManager.DeleteData();
        }

        EditorUtility.SetDirty(target);
    }
}
