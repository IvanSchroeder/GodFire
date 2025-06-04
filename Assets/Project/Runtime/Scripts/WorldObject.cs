using System;
using System.Linq;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using UnityUtilities;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel;

// https://www.reddit.com/r/Unity3D/comments/fdc2on/easily_generate_unique_ids_for_your_game_objects/

// [Serializable]
// public class WorldObjectData<T> where T : WorldObjectData {
//     [SerializeField] string _name;
//     [SerializeField] UniqueID _id = new();
//     [SerializeField] ObjectPlacementData _objectPlacementData = new();

//     public string Name {
//         get => _name;
//         set => _name = value;
//     }

//     public UniqueID ID {
//         get => _id;
//         set => _id = value;
//     }

//     public ObjectPlacementData ObjectPlacementData {
//         get => _objectPlacementData;
//         set => _objectPlacementData = value;
//     }

//     public WorldObjectSaveData() {

//     }
// }

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
public class CampfireData : WorldObjectData {
    public int CampfireCurrentHealth;
    public int CampfireCurrentFuel;
}

[Serializable]
public class TreeData : WorldObjectData {
    public int TreeCurrentHealth;
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
public abstract class WorldObject : MonoBehaviour {
    [SerializeReference] public WorldObjectData worldObjectData;
    public SpriteRenderer SpriteRenderer;

    public long ID {
        get => worldObjectData.ID.Value;
        set => worldObjectData.ID.Value = value;
    }

    public int ObjectID {
        get => worldObjectData.ObjectPlacementData.ObjectID;
        set => worldObjectData.ObjectPlacementData.ObjectID = value;
    }
    
    public virtual void Awake() {}

    public virtual void Init() {}
    
    public void SetObjectData<T>(T objectData) where T : WorldObjectData {
        worldObjectData = objectData;
    }

    public T GetObjectData<T>() where T : WorldObjectData {
        return (T)worldObjectData;
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
