using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SaveSystem {
    public class JsonDataService : IDataService {
        public readonly string fileSuffix = ".json";

        public bool SaveData<T>(T Data, string directory, string relativePath) {
            string path = SetDataPath(directory, relativePath, fileSuffix);

            try {
                if (CheckData(path.Replace(fileSuffix, string.Empty))) {
                    Debug.Log("Data exists, Deleting old file and writing a new one!");
                    File.Delete(path);
                }
                else {
                    Debug.Log("Writing file for the first time!");
                }

                // using FileStream stream = File.Create(path);
                // stream.Close();
                JsonSerializerSettings settings = new JsonSerializerSettings() {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.All
                };

                string json = JsonConvert.SerializeObject(Data, Formatting.Indented, settings);
                File.WriteAllText(path, json);

            #if UNITY_EDITOR
                AssetDatabase.Refresh();
            #endif

                return true;
            }
            catch (Exception e) {
                Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
                return false;
            }
        }

        public T LoadData<T>(string directory, string relativePath) {
            #if UNITY_EDITOR
                AssetDatabase.Refresh();
            #endif
            
            string path = SetDataPath(directory, relativePath, fileSuffix);

            if (!CheckData(path.Replace(fileSuffix, string.Empty))) {
                Debug.LogError($"Cannot load file at {path}. File does not exist!");
                throw new FileNotFoundException($"{path} does not exist!");
            }

            try {
                JsonSerializerSettings settings = new JsonSerializerSettings() {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    TypeNameHandling = TypeNameHandling.All
                };

                T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path), settings);
                return data;
            }
            catch (Exception e) {
                Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
                throw e;
            }
        }

        public bool DeleteData(string directory, string relativePath) {
            string path = SetDataPath(directory, relativePath, fileSuffix);

            if (!CheckData(path.Replace(fileSuffix, string.Empty))) {
                Debug.LogError($"Cannot delete file at {path}. File does not exist!");
                return false;
                throw new FileNotFoundException($"{path} does not exist!");
            }

            try {
                Debug.Log("Data exists, Deleting old file!");
                File.Delete(path);
                #if UNITY_EDITOR
                AssetDatabase.Refresh();
                #endif
                return true;
            }
            catch (Exception e) {
                Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
                return false;
                throw e;
            }
        }

        public bool CheckData(string path) {
            return File.Exists(path + fileSuffix);
        }

        string SetDataPath(string directory, string fileName, string fileSuffix) {
            return directory + fileName + fileSuffix;
            // return Path.Combine(directory, fileName, fileSuffix);
        }
    }
}