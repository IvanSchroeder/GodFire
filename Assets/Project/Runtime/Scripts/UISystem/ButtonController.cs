using UnityEngine;
using UnityEngine.UI;
using UnityUtilities;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonController : MonoBehaviour, IHoverable {
    public Button buttonComponent;
    public SquashAndStretchController squashAndStretchController;

    public void OnPointerEnter(PointerEventData eventData) {
        squashAndStretchController.ScaleToTarget();
    }

    public void OnPointerExit(PointerEventData eventData) {
        squashAndStretchController.ResetToInitialScale();
    }

    void Awake() {
        if (buttonComponent.IsNull()) buttonComponent = GetComponent<Button>();
    }
}