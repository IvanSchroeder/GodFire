using System;
using UnityEngine;
using UnityUtilities;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

// https://www.reddit.com/r/Unity3D/comments/fdc2on/easily_generate_unique_ids_for_your_game_objects/
[Serializable]
public class UniqueID {
    public long Value = 0;
}

[Serializable]
public class ObjectPlacementData {
    [SerializeField] private int _objectID = -1;
    [SerializeField] private Vector3Int _originalPosition = Vector3Int.zero;
    [SerializeField] private Vector2Int _size = Vector2Int.one;
    [SerializeField] private List<Vector3Int> _occupiedPositionsList = new();

    [JsonIgnore] public int ObjectID { get => _objectID; set => _objectID = value; }
    [JsonIgnore] public Vector3Int OriginalPosition { get => _originalPosition; set => _originalPosition = value; }
    [JsonIgnore] public Vector2Int Size { get => _size; set => _size = value; }
    [JsonIgnore] public List<Vector3Int> OccupiedPositionsList { get => _occupiedPositionsList; set => _occupiedPositionsList = value; }

    public ObjectPlacementData() {}

    public ObjectPlacementData(int _objectId, Vector3Int _position, Vector2Int _size, List<Vector3Int> _occupiedPositions) {
        _objectID = _objectId;
        _originalPosition = _position;
        this._size = _size;
        _occupiedPositionsList = _occupiedPositions;
    }
}

[Serializable]
public class RockData : WorldObjectData {
    public int RockCurrentHealth;
}

[Serializable]
public class ObjectData {
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public Vector2Int Size { get; private set; } = Vector2Int.one;
    [field: SerializeField] public GameObject Prefab { get; private set; }
}

[Serializable]
public class WorldObjectData {
    [SerializeField] string _name = "";
    [SerializeField] UniqueID _id = new();
    [SerializeField] ObjectPlacementData _objectPlacementData = new();

    public string Name {
        get => _name;
        set => _name = value;
    }

    public UniqueID ID {
        get => _id;
        set => _id = value;
    }

    public ObjectPlacementData ObjectPlacementData {
        get => _objectPlacementData;
        set => _objectPlacementData = value;
    }

    public WorldObjectData() {}
}

[Serializable]
public abstract class WorldObject : MonoBehaviour {
    [SerializeReference] public WorldObjectData worldObjectData;
    public SpriteRenderer SpriteRenderer;
    public Material ObjectMaterial;
    public WorldObjectShaderDataSO ShaderDataSO;

    protected Material SpriteMaterial { get => SpriteRenderer.sharedMaterial; set => SpriteRenderer.sharedMaterial = value; }

    public long ID {
        get => worldObjectData.ID.Value;
        set => worldObjectData.ID.Value = value;
    }

    public int ObjectID {
        get => worldObjectData.ObjectPlacementData.ObjectID;
        set => worldObjectData.ObjectPlacementData.ObjectID = value;
    }

    public static readonly string ON_INTERACTED = "OnInteracted";
    public static readonly string ON_DESTROYED = "OnDestroyed";

    protected int MAIN_COLOR = Shader.PropertyToID("_MainColor");
    protected int ALPHA = Shader.PropertyToID("_Alpha");

    protected int PIXELATION = Shader.PropertyToID("_Pixelation");
    protected int PIXEL_RESOLUTION = Shader.PropertyToID("_PixelResolution");

    protected int HIT_EFFECT = Shader.PropertyToID("_HitEffect");
    protected int HIT_EFFECT_COLOR = Shader.PropertyToID("_HitEffectColor");
    protected int HIT_EFFECT_GLOW = Shader.PropertyToID("_HitEffectGlow");
    protected int HIT_EFFECT_BLEND = Shader.PropertyToID("_HitEffectBlend");

    protected int EMISSION = Shader.PropertyToID("_Emission");
    protected int EMISSION_MULTIPLIER = Shader.PropertyToID("_EmissionMultiplier");

    protected int CONTRAST = Shader.PropertyToID("_Contrast");
    protected int CONTRAST_AMOUNT = Shader.PropertyToID("_ContrastAmount");

    protected int BRIGHTNESS = Shader.PropertyToID("_Brightness");
    protected int BRIGHTNESS_AMOUNT = Shader.PropertyToID("_BrightnessAmount");

    protected int SATURATION = Shader.PropertyToID("_Saturation");
    protected int SATURATION_AMOUNT = Shader.PropertyToID("_SaturationAmount");
    
#if UNITY_EDITOR
    [ExecuteInEditMode]
    protected virtual void OnValidate() {
        if (ObjectMaterial.IsNotNull()) SpriteMaterial = new Material(ObjectMaterial);
        SetShaderProperties();

        if (Application.isPlaying) return;
    }
#endif

    protected virtual void Awake() {
        if (ObjectMaterial.IsNotNull()) SpriteMaterial = new Material(ObjectMaterial);
        SetShaderProperties();
    }

    public virtual void Init() {}
    
    public void SetObjectData<T>(T objectData) where T : WorldObjectData {
        worldObjectData = objectData;
    }

    public T GetObjectData<T>() where T : WorldObjectData {
        return (T)(worldObjectData.IsNotNull() ? worldObjectData : new WorldObjectData());
    }

    public void SetShaderProperties() {
        if (ShaderDataSO.IsNull()) return;

        SpriteMaterial.SetColor(MAIN_COLOR, ShaderDataSO.mainColor);
        SpriteMaterial.SetFloat(ALPHA, ShaderDataSO.alpha);

        SpriteMaterial.SetInt(PIXELATION, ShaderDataSO.pixelation.ToInt());
        SpriteMaterial.SetFloat(PIXEL_RESOLUTION, ShaderDataSO.pixelResolution);

        SpriteMaterial.SetInt(HIT_EFFECT, ShaderDataSO.hitEffect.ToInt());
        SpriteMaterial.SetColor(HIT_EFFECT_COLOR, ShaderDataSO.SpriteFlashSettingsDictionary.GetValueOrDefault("DEFAULT").flashColor);
        SpriteMaterial.SetFloat(HIT_EFFECT_GLOW, ShaderDataSO.SpriteFlashSettingsDictionary.GetValueOrDefault("DEFAULT").flashGlow);
        SpriteMaterial.SetFloat(HIT_EFFECT_BLEND, 0);

        SpriteMaterial.SetInt(EMISSION, ShaderDataSO.emission.ToInt());
        SpriteMaterial.SetFloat(EMISSION_MULTIPLIER, ShaderDataSO.emissionMultiplier);

        SpriteMaterial.SetInt(CONTRAST, ShaderDataSO.contrast.ToInt());
        SpriteMaterial.SetFloat(CONTRAST_AMOUNT, ShaderDataSO.contrastAmount);

        SpriteMaterial.SetInt(BRIGHTNESS, ShaderDataSO.brightness.ToInt());
        SpriteMaterial.SetFloat(BRIGHTNESS_AMOUNT, ShaderDataSO.brightnessAmount);

        SpriteMaterial.SetInt(SATURATION, ShaderDataSO.saturation.ToInt());
        SpriteMaterial.SetFloat(SATURATION_AMOUNT, ShaderDataSO.saturationAmount);
    }

    public async UniTask SpriteFlash(SpriteFlashSettings flashSettings) {
        int frameCount = flashSettings.flashMaxFrames;

        SpriteMaterial.SetInt(HIT_EFFECT, true.ToInt());
        SpriteMaterial.SetFloat(HIT_EFFECT_BLEND, 1);
        SpriteMaterial.SetColor(HIT_EFFECT_COLOR, flashSettings.flashColor);

        while (frameCount >= 0) {
            if (flashSettings.blendFlash) SpriteMaterial.SetFloat(HIT_EFFECT_BLEND, frameCount.PercentageOf(flashSettings.flashMaxFrames).Clamp01());
            frameCount--;
            await UniTask.NextFrame();
        }

        SpriteMaterial.SetInt(HIT_EFFECT, false.ToInt());
        SpriteMaterial.SetFloat(HIT_EFFECT_BLEND, 0);

        await UniTask.NextFrame();
    }
    
    #if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(UniqueID))]
    private class UniqueIdDrawer : UnityEditor.PropertyDrawer {
        private const float buttonWidth = 120;
        private const float padding = 2;

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label) {
            UnityEditor.EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = UnityEditor.EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            GUI.enabled = false;
            Rect valueRect = position;
            valueRect.width -= buttonWidth + padding;
            UnityEditor.SerializedProperty idProperty = property.FindPropertyRelative("Value");
            UnityEditor.EditorGUI.PropertyField(valueRect, idProperty, GUIContent.none);

            GUI.enabled = true;

            Rect buttonRect = position;
            buttonRect.x += position.width - buttonWidth;
            buttonRect.width = buttonWidth;
            if (GUI.Button(buttonRect, "Copy to clipboard")) {
                UnityEditor.EditorGUIUtility.systemCopyBuffer = idProperty.stringValue;
            }

            UnityEditor.EditorGUI.EndProperty();
        }
    }
    #endif
}
