using UnityEngine;
using GD.MinMaxSlider;

[CreateAssetMenu(fileName = "NewHeightMapSettings", menuName = "Data/Map/Height Map Settings")]
public class HeightMapSettings : ScriptableObject {
    [Header("Noise Parameters")]
    [Space(5f)]
    public NoiseSettings noiseSettings;
    public RegionData regionData;

    [Space(10f)]
    [Header("Falloff Parameters")]
    [Space(5f)]

    public FalloffSettings falloffSettings;

    #if UNITY_EDITOR
    void OnValidate() {
        noiseSettings.ValidateValues();
    }
    #endif
}
