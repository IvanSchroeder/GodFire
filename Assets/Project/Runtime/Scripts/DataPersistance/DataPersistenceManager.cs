using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityUtilities;
using System.Linq;
using System;
using System.Collections;
using UnityEditor;

namespace SaveSystem {
    public class DataPersistenceManager : Singleton<DataPersistenceManager> {
    #if UNITY_EDITOR
        public static readonly string SAVES_FOLDER = Application.dataPath + "/Saves";
    #else
        public static readonly string SAVES_FOLDER = Application.persistentDataPath + "/Saves";
    #endif

        List<IDataHandler> DataHandlersList = new List<IDataHandler>();
        JsonDataService DataService = new JsonDataService();
        public string selectedProfileId = string.Empty;

    #if UNITY_EDITOR
        void OnValidate() {
            if (Application.isPlaying) return;

            if (DataHandlersList.IsNullOrEmpty()) {
                DataHandlersList = FindAllDataHandlers();
                Debug.Log($"Data Handlers: {DataHandlersList.Count}");
            }
        }
    #endif

        protected override void Awake() {
            base.Awake();

            Init();
        }

        void Start() {
            DataHandlersList = FindAllDataHandlers();
            Debug.Log($"Data Handlers: {DataHandlersList.Count}");

            LoadGameData();
        }

        public void Init() {
            CreateDirectory(SAVES_FOLDER);

            if (DataService.IsNull()) {
                DataService = new JsonDataService();
            }
        }

        public void CreateDirectory(string path) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
                AssetDatabase.Refresh();
            }
        }

        public void DeleteDirectory(string path) {
            // if (Directory.Exists(path)) {
            //     DirectoryInfo directoryInfo = new DirectoryInfo(path);
            //     var files = directoryInfo.GetFiles("*.*");

            //     foreach (FileInfo file in files) {
            //         file.Delete();
            //     }

            //     directoryInfo.Delete(true);

            //     // Directory.Delete(path, true);

            //     AssetDatabase.Refresh();
            // }

            try {
                if (File.Exists(path)) {
                    Directory.Delete(Path.GetDirectoryName(path), true);
                }
                else {
                    Debug.LogWarning($"Tried to delete profile data, but data was not found at path: {path}");                }
            }
            catch (Exception e) {
                Debug.LogWarning($"Failed to delete profile data for prifleId: {selectedProfileId} at path: {path} \n{e}");  
            }

            AssetDatabase.Refresh();
        }

        public void CreateGameData() {
            foreach (IDataHandler dataHandler in DataHandlersList) {
                dataHandler.CreateData();
            }
        }

        public void SaveGameData() {
            foreach (IDataHandler dataHandler in DataHandlersList) {
                dataHandler.SaveData();
            }
        }

        public void LoadGameData() {
            foreach (IDataHandler dataHandler in DataHandlersList) {
                dataHandler.LoadData();
            }
        }

        public void DeleteGameData() {
            foreach (IDataHandler dataHandler in DataHandlersList) {
                dataHandler.DeleteData();
            }
        }

        public void WriteData<T>(T data) {
            string directoryPath = SAVES_FOLDER + $"/{selectedProfileId}";

            CreateDirectory(directoryPath);

            if (DataService.SaveData(data, directoryPath, $"/{typeof(T).Name}")) {
                Debug.Log("File saved successfully!");
            }
            else {
                Debug.LogError("Could not save file!");
            }
        }

        public T ReadData<T>(T data) {
            string directoryPath = SAVES_FOLDER + $"/{selectedProfileId}";
            return DataService.LoadData<T>(directoryPath, $"/{typeof(T).Name}");
        }

        public bool ClearData<T>(T data) {
            string directoryPath = SAVES_FOLDER + $"/{selectedProfileId}";
            return DataService.DeleteData(directoryPath, $"/{typeof(T).Name}");
        }

        public bool CheckData<T>(T data) {
            string directoryPath = SAVES_FOLDER + $"/{selectedProfileId}";
            return DataService.CheckData(directoryPath + $"/{typeof(T).Name}");
        }

        List<IDataHandler> FindAllDataHandlers() {
            IEnumerable<IDataHandler> dataHandlers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataHandler>();

            return new List<IDataHandler>(dataHandlers);
        }

        // void OnApplicationQuit() {
        //     SaveGameData();
        // }
    }
}