using UnityEngine;
using UnityUtilities;
using GD.MinMaxSlider;


public static class MyNoise {
    public enum NormalizeMode { Local, Global };

    public static float RemapValue(float value, float initialMin, float initialMax, float outputMin, float outputMax) {
        return outputMin + (value - initialMin) * (outputMax - outputMin) / (initialMax - initialMin);
    }

    public static float RemapValue01(float value, float outputMin, float outputMax) {
        return outputMin + (value - 0) * (outputMax - outputMin) / (1 - 0);
    }

    public static int RemapValue01ToInt(float value, float outputMin, float outputMax) {
        return RemapValue01(value, outputMin, outputMax).ToIntRound();
    }

    public static float Redistribution(float noise, NoiseSettings settings) {
        return (noise * settings.redistributionModifier).PowerOf(settings.exponent);
    }

    public static float OctavePerlin(float x, float y, NoiseSettings settings) {
        x *= settings.scale;
        y *= settings.scale;
        x += settings.scale;
        y += settings.scale;
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float amplitudeSum = 0;

        for (int i = 0; i < settings.octaves; i++) {
            total += Mathf.PerlinNoise(
                (settings.offset.x + x) * frequency,
                (settings.offset.y + y) * frequency)
                * amplitude;
            amplitudeSum += amplitude;

            amplitude *= settings.persistance;
            frequency *= 2;
        }

        return total / amplitudeSum;
    }

    // Sebastian Lague https://www.youtube.com/watch?v=WP-Bm65Q-1Y&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3&index=16
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCenter) {
        float[,] noiseMap = new float[mapWidth,mapHeight];

        Random.InitState(settings.seed);
        Vector2[] octaveOffsets = new Vector2[settings.octaves];

        float maxPossibleHeight = 0;
        float amplitude = settings.amplitude;
        float frequency = settings.frequency;

        for (int i = 0; i < settings.octaves; i++) {
            float xOffset = Random.Range(-100000, 100000) * (mapWidth / settings.scale);
            float yOffset = Random.Range(-100000, 100000) * (mapHeight / settings.scale);

            octaveOffsets[i] = new Vector2(xOffset / mapWidth, yOffset / mapHeight);

            maxPossibleHeight += amplitude;
            amplitude *= settings.persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        int halfWidth = mapWidth / 2;
        int halfHeight = mapHeight / 2;

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                amplitude = settings.amplitude;
                frequency = settings.frequency;
                float noiseHeight = 0;
                float superpositionCompensation = 0;

                for (int i = 0; i < settings.octaves; i++) {
                    float sampleX = (x - halfWidth - settings.offset.x + sampleCenter.x) / settings.scale * frequency + (octaveOffsets[i].x * frequency);
                    float sampleY = (y - halfHeight - settings.offset.y + sampleCenter.y) / settings.scale * frequency + (octaveOffsets[i].y * frequency);

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                    noiseHeight += perlinValue * amplitude;
                    noiseHeight -= superpositionCompensation;

                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;
                    superpositionCompensation = amplitude / 2;
                }

                if (noiseHeight > maxLocalNoiseHeight) {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight) {
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x,y] = noiseHeight;

                if (settings.normalizeMode == NormalizeMode.Global) {
                    // float normalizedHeight = (noiseMap[x,y] + 1) / (maxPossibleHeight / 0.9f);
                    // float normalizedHeight = (noiseMap[x,y] + 1) / (maxPossibleHeight);
                    float normalizedHeight = noiseMap[x,y];
                    noiseMap[x,y] = normalizedHeight.Clamp(0, int.MaxValue).RoundTo(3);
                }
            }
        }

        if (settings.normalizeMode == NormalizeMode.Local) {
            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++) {
                    noiseMap[x,y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x,y]).RoundTo(3);
                }
            }
        }

        return noiseMap;
    }

    public static float[,] GenerateFalloffMap(int width, int height, HeightMapSettings heightMapSettings) {
        float[,] falloffMap = new float[width,height];

        float offsetFalloffX = 1;
        float offsetFalloffY = 1;

        float steepness;
        float shift;

        if (heightMapSettings.FalloffSettings.randomizeFalloff) {
            Random.InitState(heightMapSettings.NoiseSettings.seed);
            steepness = Random.Range(heightMapSettings.FalloffSettings.steepnessRange.x, heightMapSettings.FalloffSettings.steepnessRange.y);
            shift = Random.Range(heightMapSettings.FalloffSettings.shiftRange.x, heightMapSettings.FalloffSettings.shiftRange.y);

            if (shift > 4) {
                offsetFalloffX = Random.Range(0.90f, 1.10f);
                offsetFalloffY = Random.Range(0.90f, 1.10f);
            } else if(shift > 3) {
                offsetFalloffX = Random.Range(0.80f, 1.19f);
                offsetFalloffY = Random.Range(0.80f, 1.19f);
            } else if(shift > 2) {
                offsetFalloffX = Random.Range(0.75f, 1.24f);
                offsetFalloffY = Random.Range(0.75f, 1.24f);
            } else if(shift >= 1) {
                offsetFalloffX = Random.Range(0.67f, 1.32f);
                offsetFalloffY = Random.Range(0.67f, 1.32f);
            }

            if (shift < 1) shift = 0;
        }
        else {
            steepness = heightMapSettings.FalloffSettings.falloffSteepness;
            shift = heightMapSettings.FalloffSettings.falloffShift;
        }

        float dist;
        float distSqr;
        float minD;
        float maxD;
        float dx;
        float dy;
        float innerRadius = heightMapSettings.FalloffSettings.innerFalloffRadius;
        float outerRadius = heightMapSettings.FalloffSettings.outerFalloffRadius;

        float iRadSqr = (innerRadius * heightMapSettings.FalloffSettings.radiusFactor).PowerOf(2);
        float oRadSqr = (outerRadius * heightMapSettings.FalloffSettings.radiusFactor).PowerOf(2);

        float value;

        // maxD = Mathf.Sqrt((width.PowerOf(2) * 0.25f) + (height.PowerOf(2) * 0.25f));
        minD = Mathf.Sqrt((width.PowerOf(2) * innerRadius * heightMapSettings.FalloffSettings.radiusFactor) + (height.PowerOf(2) * innerRadius * heightMapSettings.FalloffSettings.radiusFactor));
        maxD = Mathf.Sqrt((width.PowerOf(2) * outerRadius * heightMapSettings.FalloffSettings.radiusFactor) + (height.PowerOf(2) * outerRadius * heightMapSettings.FalloffSettings.radiusFactor));

        for (int j = 0; j < height; j++) {
            for (int i = 0; i < width; i++) {
                dx = width / 2 - i - offsetFalloffX;
                dy = height / 2 - j - offsetFalloffY;
                distSqr = dx.PowerOf(2) + dy.PowerOf(2);
                dist = Mathf.Sqrt(distSqr);

                value = (-(outerRadius - (2 * dist / maxD))).Clamp01();

                // float innerValue = (-(innerRadius - (2 * dist / minD))).Clamp01();
                // float outerValue = (-(outerRadius - (2 * dist / maxD))).Clamp01();

                // value = -((outerValue * 2) - innerValue);
                
                value = FalloffEvaluation(value, steepness, shift).Clamp01();

                // if (distSqr >= oRadSqr) value = 1f;
                // if (distSqr <= iRadSqr) value = 0f;

                // if (distSqr <= iRadSqr) value = 0f;

                // float t = Mathf.Lerp(0, 1, dist);

                // if (distSqr >= oRadSqr) value = 1;
                // else if (distSqr <= iRadSqr) value = 0;
                // else {
                //     float t = Mathf.Lerp(innerRadius, outerRadius, dist);
                //     value = (1 - (2 * dist / maxD)) * -1 * t;
                //     value = FalloffEvaluation(value, steepness, shift).Clamp01();
                // }

                // float x = i / (float)width * 2 - offsetFalloffX;
                // float y = j / (float)height * 2 - offsetFalloffY;
                // float value = Mathf.Max(x.AbsoluteValue(), y.AbsoluteValue());

                // switch (heightMapSettings.falloffSettings.falloffMode) {
                //     case FalloffSettings.FalloffMode.Squared:
                //         falloffMap[i,j] = FalloffEvaluation(value, steepness, shift);
                //         break;
                //     case FalloffSettings.FalloffMode.SquaredCurve:
                //         falloffMap[i,j] = heightMapSettings.falloffSettings.fallofRedistributionCurve.Evaluate(value);
                //         break;
                //     case FalloffSettings.FalloffMode.Radial:
                //         falloffMap[i,j] = RadialFalloff(value, heightMapSettings.falloffSettings.outerFalloffRadius, i, j, width / 2, height / 2);
                //         break;
                //     case FalloffSettings.FalloffMode.SmoothRadial:
                //         falloffMap[i,j] = SmoothRadialFalloff(value, heightMapSettings.falloffSettings.innerFalloffRadius, heightMapSettings.falloffSettings.outerFalloffRadius, i, j, width / 2, height / 2);
                //         break;
                // }

                falloffMap[i,j] = value.RoundTo(3).Clamp01();
            }
        }

        return falloffMap;
    }

    public static float FalloffEvaluation(float value, float a = 3, float b = 2.2f) {
        float n1 = value.PowerOf(a);
        float n2 = (b - b * value).PowerOf(a);
        return n1 / (n1 + n2);
    }

    public static float RadialFalloff(float value, float radius, int x, int y, float cx, float cy) {
        float dx = cx - x;
        float dy = cy - y;
        float distSqr = dx.PowerOf(2) + dy.PowerOf(2);
        float radSqr = radius.PowerOf(2);

        if (distSqr > radSqr) return 1f;
        return value;
    }

    public static float SmoothRadialFalloff(float value, float innerRadius, float outerRadius, int x, int y, float cx, float cy) {
        float dx = cx - x;
        float dy = cy - y;
        float distSqr = dx.PowerOf(2) + dy.PowerOf(2);
        float iRadSqr = innerRadius.PowerOf(2);
        float oRadSqr = outerRadius.PowerOf(2);

        if (distSqr >= oRadSqr) return 1f;
        if (distSqr <= iRadSqr) return 0f;

        float dist = Mathf.Sqrt(distSqr);
        float t = Mathf.Lerp(innerRadius, outerRadius, dist);

        return value * t;
    }
}

[System.Serializable]
public class NoiseSettings {
    public MyNoise.NormalizeMode normalizeMode;

    public int seed;
    public Vector2Int offset;

    [Range(0.01f, 256)] public float scale = 32;
    [Range(1, 15)] public int octaves = 4;
    [Range(0, 16)] public float amplitude = 4;
    [Range(0, 10)] public float frequency = 0.05f;
    [Range(0, 1)] public float persistance = 0.5f;
    [Min(1)] public float lacunarity = 2;
    [Range(0, 10)] public float heightMultiplier = 1;
    public AnimationCurve heightCurve;

    public float redistributionModifier;
    public float exponent;

    public void ValidateValues() {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = persistance.Clamp01();
    }
}

[System.Serializable]
public class FalloffSettings {
    public enum FalloffMode { Squared, SquaredCurve, Radial, SmoothRadial };

    public bool useFalloffMap = true;
    public FalloffMode falloffMode = FalloffMode.Squared;
    [Range(0, 10)] public float falloffFactor = 1;
    public bool useCurve = true;
    public AnimationCurve fallofRedistributionCurve = new AnimationCurve();
    [Range(0, 20)] public float falloffSteepness = 3;
    [Range(0, 50)] public float falloffShift = 2.2f;
    public bool randomizeFalloff = false;
    [MinMaxSlider(1, 20)] public Vector2 steepnessRange;
    [MinMaxSlider(1, 50)] public Vector2 shiftRange;
    public bool useRadialFalloff = false;
    [Range(0, 1)] public float outerFalloffRadius = 0;
    [Range(0, 1)] public float innerFalloffRadius = 1;
    [Range(0, 1)] public float radiusFactor = 0.25f;
    // [Range(0, 1)] public float radialFalloffRadius;
}
