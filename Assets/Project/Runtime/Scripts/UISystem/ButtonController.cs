using UnityEngine;
using UnityEngine.UI;
using UnityUtilities;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(Button))]
public class ButtonController : MonoBehaviour, IHoverable, IPointerClickHandler {
    public Button buttonComponent;
    public SquashAndStretchController squashAndStretchController;

    public static string BUTTON_PRESS = "BUTTON_PRESS";
    public static string BUTTON_HOVER = "BUTTON_HOVER";

    public void OnPointerEnter(PointerEventData eventData) {
        squashAndStretchController.PlaySquashAndStretch(UIManager.Instance.SquashStretchSettingsDictionary.GetValueOrDefault(BUTTON_HOVER));
    }

    public void OnPointerExit(PointerEventData eventData) {
        squashAndStretchController.ResetToInitialScale();
    }

    public void OnPointerClick(PointerEventData eventData) {
        squashAndStretchController.PlaySquashAndStretch(UIManager.Instance.SquashStretchSettingsDictionary.GetValueOrDefault(BUTTON_PRESS));
    }

    void Awake() {
        if (buttonComponent.IsNull()) buttonComponent = GetComponent<Button>();
    }
}