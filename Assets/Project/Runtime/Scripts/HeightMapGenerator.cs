using UnityEngine;
using UnityUtilities;

public static class HeightMapGenerator {
    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings heightMapSettings, Vector2 sampleCenter) {
        float[,] values = MyNoise.GenerateNoiseMap(width, height, heightMapSettings.noiseSettings, sampleCenter);
        float[,] falloffMap = MyNoise.GenerateFalloffMap(width, height, heightMapSettings);
        // Color[] colorMap = new Color[width * height];

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;
        AnimationCurve heightCurve_threadSafe = new AnimationCurve(heightMapSettings.noiseSettings.heightCurve.keys);

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                values[x,y] = ((values[x,y] - (heightMapSettings.falloffSettings.useFalloffMap ? (falloffMap[x,y] * heightMapSettings.falloffSettings.falloffFactor) : 0))
                    * heightCurve_threadSafe.Evaluate(values[x,y]) * heightMapSettings.noiseSettings.heightMultiplier).Clamp01().RoundTo(3);
                
                if (values[x,y] > maxValue) {
                    maxValue = values[x,y];
                }
                if (values[x,y] < minValue) {
                    minValue = values[x,y];
                }
            }
        }

        return new HeightMap(values, minValue, maxValue, falloffMap);
    }

    public static Color[] GenerateColorMap(int width, int height, float[,] values, HeightMapSettings heightMapSettings) {
        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                for (int i = 0; i < heightMapSettings.regionData.RegionsList.Count; i++) {
                    float currentHeight = values[x,y].Clamp01();

                    if (currentHeight <= heightMapSettings.regionData.RegionsList.GetElement(i).height) {
                        colorMap[y * width + x] = heightMapSettings.regionData.RegionsList.GetElement(i).color;
                        break;
                    }
                    else {
                        continue;
                    }
                }
            }
        }

        return colorMap;
    }

    public static float[,] GetMapValuesAt(float[,] worldValues, Vector2Int chunkLocalPos, int chunkSize) {
        float[,] mapValues = new float[chunkSize,chunkSize];

        int totalTilesW = worldValues.GetLength(0);
        int totalTilesH = worldValues.GetLength(1);
        int totalChunksW = totalTilesW / chunkSize;
        int totalChunksH = totalTilesH / chunkSize;
        int halfWidthInChunks = (totalChunksW / 2);
        int halfHeightInChunks = (totalChunksH / 2);

        // Vector2Int sampleCenter = new Vector2Int(chunkLocalPos.x + halfWidthInChunks, chunkLocalPos.y + halfHeightInChunks);
        Vector2Int sampleCenter = chunkLocalPos;
        // Debug.Log(sampleCenter);

        if ((sampleCenter.x * chunkSize > totalTilesW) || (sampleCenter.y * chunkSize > totalTilesH) || (sampleCenter.x < 0) || (sampleCenter.y < 0)) {
            return mapValues;
        }
        
        for (int y = 0; y < chunkSize; y++) {
            for (int x = 0; x < chunkSize; x++) {
                mapValues[x,y] = worldValues[(sampleCenter.x * chunkSize) + x, (sampleCenter.y * chunkSize) + y];
            }
        }

        return mapValues;
    }
}

[System.Serializable]
public struct HeightMap {
    public float[,] values;
    public float minValue;
    public float maxValue;
    public float[,] falloffMap;

    public HeightMap(float[,] _values = null, float _minValue = 0, float _maxValue = 0, float[,] _falloffMap = null) {
        values = _values;
        minValue = _minValue;
        maxValue = _maxValue;
        falloffMap = _falloffMap;
    }
}