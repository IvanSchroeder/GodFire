using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityUtilities;

[CreateAssetMenu(fileName = "NewShaderData", menuName = "Data/Settings/Shader Data/World Object")]
public class WorldObjectShaderDataSO : ScriptableObject {
    [Header("Global")]
    [Space(2)]
    public Color mainColor = Color.white;
    public float alpha = 1;
    
    [Space(5)]
    [Header("Pixelation")]
    [Space(2)]
    public bool pixelation = false;
    public int pixelResolution = 1280;

    [Space(5)]
    [Header("Sprite Flash")]
    [Space(2)]
    public bool hitEffect = false;
    public SerializedDictionary<string, SpriteFlashSettings> SpriteFlashSettingsDictionary = new() {
        { "DEFAULT", new SpriteFlashSettings(Color.white, 1, true, 30) }
    };

    [Space(5)]
    [Header("Emission")]
    [Space(2)]
    public bool emission = false;
    public float emissionMultiplier = 1;

    [Space(5)]
    [Header("Contrast")]
    [Space(2)]
    public bool contrast = false;
    public float contrastAmount = 1;

    [Space(5)]
    [Header("Brightness")]
    [Space(2)]
    public bool brightness = false;
    public float brightnessAmount = 1;

    [Space(5)]
    [Header("Saturation")]
    public bool saturation = false;
    public float saturationAmount = 1;

    [Space(5)]
    [Header("Object Punch")]
    [Space(2)]
    public float punchRotationAmount = 5;
    public float punchRotationDuration = 0.75f;
    public int punchRotationVibrato = 3;
    public float punchRotationElasticity = 5;

    public SpriteFlashSettings GetSpriteFlashSettings(string key) {
        return SpriteFlashSettingsDictionary.GetValueOrDefault(key, SpriteFlashSettingsDictionary["DEFAULT"]);
    }
}

[Serializable]
public class SpriteFlashSettings {
    public Color flashColor;
    public float flashGlow;
    public bool blendFlash;
    public int flashMaxFrames;

    public SpriteFlashSettings(Color _flashColor, float _flashGlow, bool _blendFlash, int _flashMaxFrames) {
        flashColor = _flashColor;
        flashGlow = _flashGlow;
        blendFlash = _blendFlash;
        flashMaxFrames = _flashMaxFrames;
    }
}
