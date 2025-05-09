using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;
using UnityUtilities;

public class ThreadedDataRequester : Singleton<ThreadedDataRequester> {
    Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();

    void Update() {
        if (dataQueue.Count > 0) {
            for (int i = 0; i < dataQueue.Count; i++) {
                ThreadInfo threadInfo = dataQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    public static void RequestData(Func<object> generateData, Action<object> callback) {
        ThreadStart threadStart = delegate {
            Instance.DataThread(generateData, callback);
        };

        new Thread(threadStart).Start();
    }

    void DataThread(Func<object> generateData, Action<object> callback) {
        object data = generateData();
        lock (dataQueue) {
            dataQueue.Enqueue(new ThreadInfo(callback, data));
        }
    }

    // public void RequestHeightMap(Vector2 center, Action<HeightMap> callback) {
    //     ThreadStart threadStart = delegate {
    //         HeightMapThread(center, callback);
    //     };

    //     new Thread(threadStart).Start();
    // }

    // void HeightMapThread(Vector2 center, Action<HeightMap> callback) {
    //     HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(mapWidth, mapHeight, heightMapSettings, center);
    //     lock (dataQueue) {
    //         dataQueue.Enqueue(new ThreadInfo<HeightMap>(callback, heightMap));
    //     }
    // }

    struct ThreadInfo {
        public readonly Action<object> callback;
        public readonly object parameter;

        public ThreadInfo(Action<object> _callback, object _parameter) {
            callback = _callback;
            parameter = _parameter;
        }
    }
}
