using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityUtilities;

public interface IInteractable {
    InteractableTrigger InteractableTrigger { get; set; }
    void OnInteract();
    void OnHoverStart();
    void OnHoverEnd();
}

public interface IHoverable : IPointerEnterHandler, IPointerExitHandler {}