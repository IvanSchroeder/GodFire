using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public static class HeightMapGenerator {
    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings heightMapSettings, Vector2 sampleCenter) {
        float[,] valuesMap = MyNoise.GenerateNoiseMap(width, height, heightMapSettings.NoiseSettings, sampleCenter);
        float[,] falloffMap = MyNoise.GenerateFalloffMap(width, height, heightMapSettings);

        if (valuesMap.IsNull()) valuesMap = GenerateEmptyMap(width, height);
        if (falloffMap.IsNull()) falloffMap = GenerateEmptyMap(width, height, 0);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;
        AnimationCurve heightCurve_threadSafe = new AnimationCurve(heightMapSettings.NoiseSettings.heightCurve.keys);

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                valuesMap[x,y] = ((valuesMap[x,y] - (heightMapSettings.FalloffSettings.useFalloffMap && falloffMap.IsNotNull() ? (falloffMap[x,y] * heightMapSettings.FalloffSettings.falloffFactor) : 0))
                    * heightCurve_threadSafe.Evaluate(valuesMap[x,y]) * heightMapSettings.NoiseSettings.heightMultiplier).Clamp01().RoundTo(3);
                
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
                Color terrainColor = Color.black;
                float heightValue = values[x,y].Clamp01();
                float previousRangeLimit = 0;
                float currentRangeLimit = 0;

                for (int i = 0; i < heightMapSettings.RegionData.RegionsList.Count; i++) {
                    previousRangeLimit = currentRangeLimit;
                    currentRangeLimit = heightMapSettings.RegionData.RegionsList.GetElement(i).height;

                    if (heightValue >= currentRangeLimit) {
                        terrainColor = heightMapSettings.RegionData.RegionsList.GetElement(i).worldTile.color;
                    }
                    else {
                        break;
                    }
                }

                colorMap[y * width + x] = terrainColor;
            }
        }

        return colorMap;
    }
    
    public static float[,] GetMapValuesAt(float[,] worldValues, Vector2Int chunkGridPos, int widthSize, int heightSize) {
        float[,] mapValues = new float[widthSize,heightSize];

        int totalTilesW = worldValues.GetLength(0);
        int totalTilesH = worldValues.GetLength(1);
        
        Vector2Int sampleCenter = chunkGridPos;

        if ((sampleCenter.x * widthSize > totalTilesW) || (sampleCenter.y * heightSize > totalTilesH) || (sampleCenter.x < 0) || (sampleCenter.y < 0)) {
            return mapValues;
        }
        
        for (int y = 0; y < heightSize; y++) {
            for (int x = 0; x < widthSize; x++) {
                mapValues[x,y] = worldValues[(sampleCenter.x * widthSize) + x, (sampleCenter.y * heightSize) + y];
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

    public static float[,] SetMapValuesAt(float[,] worldValues, float[,] overwriteValues, Vector2Int chunkGridPos, int widthSize, int heightSize) {
        float[,] modifiedMapValues = worldValues;

        Vector2Int sampleCenter = chunkGridPos;
        int i = 0;
        int j = 0;

        for (int y = 0; y < heightSize; y++) {
            for (int x = 0; x < widthSize; x++) {
                i = (sampleCenter.x * widthSize) + x;
                j = (sampleCenter.y * heightSize) + y;

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