using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public static class HeightMapGenerator {
    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings heightMapSettings, Vector2 sampleCenter) {
        float[,] valuesMap = MyNoise.GenerateNoiseMap(width, height, heightMapSettings.noiseSettings, sampleCenter);
        float[,] falloffMap = MyNoise.GenerateFalloffMap(width, height, heightMapSettings);

        if (valuesMap.IsNull()) valuesMap = GenerateEmptyMap(width, height);
        if (falloffMap.IsNull()) falloffMap = GenerateEmptyMap(width, height, 0);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;
        AnimationCurve heightCurve_threadSafe = new AnimationCurve(heightMapSettings.noiseSettings.heightCurve.keys);

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                valuesMap[x,y] = ((valuesMap[x,y] - (heightMapSettings.falloffSettings.useFalloffMap && falloffMap.IsNotNull() ? (falloffMap[x,y] * heightMapSettings.falloffSettings.falloffFactor) : 0))
                    * heightCurve_threadSafe.Evaluate(valuesMap[x,y]) * heightMapSettings.noiseSettings.heightMultiplier).Clamp01().RoundTo(3);
                
                if (valuesMap[x,y] > maxValue) {
                    maxValue = valuesMap[x,y];
                }
                if (valuesMap[x,y] < minValue) {
                    minValue = valuesMap[x,y];
                }
            }
        }

        return new HeightMap(valuesMap, minValue, maxValue, falloffMap);
    }

    public static float[,] GenerateEmptyMap(int width, int height, float defaultValue = 0) {
        float[,] emptyMap = new float[width,height];

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                emptyMap[x,y] = defaultValue;
            }
        }

        return emptyMap;
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
    
    public static float[,] GetMapValuesAt(float[,] worldValues, Vector2Int chunkGridPos, int chunkSize) {
        float[,] mapValues = new float[chunkSize,chunkSize];

        int totalTilesW = worldValues.GetLength(0);
        int totalTilesH = worldValues.GetLength(1);
        int totalChunksW = totalTilesW / chunkSize;
        int totalChunksH = totalTilesH / chunkSize;
        int halfWidthInChunks = (totalChunksW / 2);
        int halfHeightInChunks = (totalChunksH / 2);

        // Vector2Int sampleCenter = new Vector2Int(chunkLocalPos.x + halfWidthInChunks, chunkLocalPos.y + halfHeightInChunks);
        Vector2Int sampleCenter = chunkGridPos;
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
    

    public static float[,] GetMapValuesInRange(float[,] worldValues, float minRange = 0, float maxRange = 1, bool minInclusive = true, bool maxInclusive = false, bool normalized = false) {
        int xDim = worldValues.GetLength(0);
        int yDim = worldValues.GetLength(1);

        float[,] rangedValues = new float[xDim,yDim];

        for (int y = 0; y < yDim; y++) {
            for (int x = 0; x < xDim; x++) {
                if ((minInclusive ? worldValues[x,y] >= minRange : worldValues[x,y] > minRange) &&
                    (maxInclusive ? worldValues[x,y] <= maxRange : worldValues[x,y] < maxRange)) {
                    rangedValues[x,y] = normalized ? 1 : worldValues[x,y];
                }
                else {
                    rangedValues[x,y] = 0;
                }
            }
        }

        return rangedValues;
    }

    public static float[,] SetMapValuesAt(float[,] worldValues, float[,] overwriteValues, Vector2Int chunkGridPos, int chunkSize) {
        float[,] modifiedMapValues = worldValues;

        Vector2Int sampleCenter = chunkGridPos;
        int i = 0;
        int j = 0;

        for (int y = 0; y < chunkSize; y++) {
            for (int x = 0; x < chunkSize; x++) {
                i = (sampleCenter.x * chunkSize) + x;
                j = (sampleCenter.y * chunkSize) + y;

                if (modifiedMapValues[i,j] != overwriteValues[i,j]) {
                    modifiedMapValues[i,j] = overwriteValues[i,j];
                }
            }
        }

        return modifiedMapValues;
    }
}

[System.Serializable]
public struct HeightMap {
    // public int xDim = 1;
    // public int yDim = 1;
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