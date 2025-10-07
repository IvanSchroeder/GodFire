using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityUtilities;
using System.Linq;
using System;
using Cysharp.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
            }
        }
    #endif

        protected override void Awake() {
            base.Awake();

            Init();
        }

        public void Init() {
            CreateDirectory(SAVES_FOLDER);

            if (DataService.IsNull()) {
                DataService = new JsonDataService();
            }
        }

        public void StartDataLoading() {
            DataHandlersList = FindAllDataHandlers();
        }

        public void CreateDirectory(string path) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            
            #if UNITY_EDITOR
                AssetDatabase.Refresh();
            #endif
            }
        }

        public void DeleteDirectory(string path) {
            try {
                if (File.Exists(path)) {
                    Directory.Delete(Path.GetDirectoryName(path), true);
                }
                else {
                    Debug.LogWarning($"Tried to delete profile data, but data was not found at path: {path}");                
                }
            }
            catch (Exception e) {
                Debug.LogWarning($"Failed to delete profile data for prifleId: {selectedProfileId} at path: {path} \n{e}");  
            }

        #if UNITY_EDITOR
            AssetDatabase.Refresh();
        #endif
        }

        public async UniTask CreateGameData(string profileId) {
            SetSelectedProfileId(profileId);
            foreach (IDataHandler dataHandler in DataHandlersList) {
                await dataHandler.CreateData();
            }

            await UniTask.Yield();
        }

        public async UniTask SaveGameData() {
            foreach (IDataHandler dataHandler in DataHandlersList) {
                await dataHandler.SaveData();
            }

            await UniTask.Yield();
        }

        public async void SaveOnMenu() {
            foreach (IDataHandler dataHandler in DataHandlersList) {
                await dataHandler.SaveData();
            }

            await UniTask.Yield();
        }

        public async UniTask LoadGameData(string profileId) {
            SetSelectedProfileId(profileId);

            foreach (IDataHandler dataHandler in DataHandlersList) {
                await dataHandler.LoadData();
            }

            await UniTask.Yield();
        }

        public async UniTask DeleteGameData(string profileId) {
            SetSelectedProfileId(profileId);

            foreach (IDataHandler dataHandler in DataHandlersList) {
                await dataHandler.DeleteData();
            }

            await UniTask.Yield();
        }

        private void SetSelectedProfileId(string profileId) {
            selectedProfileId = profileId;
            Debug.Log($"Set Selected Profile ID : {selectedProfileId}");
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

        private List<IDataHandler> FindAllDataHandlers() {
            IEnumerable<IDataHandler> dataHandlers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataHandler>();

            return new List<IDataHandler>(dataHandlers);
        }

        // void OnApplicationQuit() {
        //     SaveGameData();
        // }
    }
}