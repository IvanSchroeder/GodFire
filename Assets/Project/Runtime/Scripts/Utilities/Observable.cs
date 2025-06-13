using System;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Events;
#endif

[Serializable]
public class Observable<T> {
    private T value;
    public event Action<T> ValueChanged;

    public T Value {
        get => value;
        set => Set(value);
    }

    public static implicit operator T(Observable<T> observable) => observable.value;

    public Observable(T _value, Action<T> onValueChanged = null) {
        value = _value;

        if (onValueChanged != null)
            ValueChanged += onValueChanged;
    }

    public void Set(T _value) {
        // if (EqualityComparer<T>.Default.Equals(this.value, _value))
        //     return;
        if (Equals(value, _value))
            return;
        value = _value;
        Invoke();
    }
    
    public void Invoke() {
        ValueChanged?.Invoke(value);
    }

    public void AddListener(Action<T> handler) {
        ValueChanged += handler;
    }

    public void RemoveListener(Action<T> handler) {
        ValueChanged -= handler;
    }

    public void Dispose() {
        ValueChanged = null;
        value = default;
    }

//     [SerializeField] T value;
//     [SerializeField] public UnityEvent<T> OnValueChanged;

//     public T Value {
//         get => value;
//         set => Set(value);
//     }

//     public static implicit operator T(Observable<T> observable) => observable.value;

//     public Observable(T value, UnityAction<T> callback = null) {
//         this.value = value;
//         OnValueChanged = new UnityEvent<T>();
//         if (callback != null) OnValueChanged.AddListener(callback);
//     }

//     public void Set(T _value) {
//         if (Equals(value, _value)) return;
//         value = _value;
//         Invoke();
//     }

//     public void Invoke() {
//         Debug.Log($"Invoking {OnValueChanged.GetPersistentEventCount()} listeners");
//         OnValueChanged.Invoke(value);
//     }

//     public void AddListener(UnityAction<T> callback) {
//         if (callback == null) return;
//         if (OnValueChanged == null) OnValueChanged = new UnityEvent<T>();

// #if UNITY_EDITOR
//         UnityEventTools.AddPersistentListener(OnValueChanged, callback);
// #else
//         OnValueChanged.AddListener(callback);
// #endif    
//     }

//     public void RemoveListener(UnityAction<T> callback) {
//         if (callback == null) return;
//         if (OnValueChanged == null) return;

// #if UNITY_EDITOR
//         UnityEventTools.RemovePersistentListener(OnValueChanged, callback);
// #else
//         OnValueChanged.RemoveListener(callback);
// #endif 
//     }

//     public void RemoveAllListeners() {
//         if (OnValueChanged == null) return;

// #if UNITY_EDITOR
//         FieldInfo fieldInfo = typeof(UnityEventBase).GetField("m_PersistentCalls", BindingFlags.Instance | BindingFlags.NonPublic);
//         object value = fieldInfo.GetValue(OnValueChanged);
//         value.GetType().GetMethod("Clear").Invoke(value, null);
// #else
//         OnValueChanged.RemoveAllListeners();
// #endif
//     }

//     public void Dispose() {
//         RemoveAllListeners();
//         OnValueChanged = null;
//         value = default;
//     }
}