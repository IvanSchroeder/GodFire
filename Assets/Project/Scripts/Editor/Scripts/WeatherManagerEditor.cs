using UnityEngine;
using UnityEditor;
using WorldSimulation;
using Cysharp.Threading.Tasks;

[CustomEditor(typeof(WeatherManager))]
public class WeatherManagerEditor : Editor {
    WeatherManager weatherManager;

    public override void OnInspectorGUI() {
        weatherManager = (WeatherManager)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Create Game Data")) {
            weatherManager.CreateData();
        }

        if (GUILayout.Button("Save Game Data")) {
            weatherManager.SaveData();
        }

        if (GUILayout.Button("Load Game Data")) {
            weatherManager.LoadData();
        }

        if (GUILayout.Button("Delete Game Data")) {
            weatherManager.DeleteData();
        }

        EditorUtility.SetDirty(target);
    }
}
