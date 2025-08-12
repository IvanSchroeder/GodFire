using UnityEngine;
using UnityUtilities;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityUtilies;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
public class ScreenController : MonoBehaviour {
    public string ScreenName = "";
    private Canvas CanvasComponent;
    private CanvasGroup CanvasGroupComponent;
    public bool isShown = false;

    void Awake() {
        if (CanvasComponent.IsNull()) CanvasComponent = GetComponent<Canvas>();
        CanvasGroupComponent = GetComponent<CanvasGroup>();
    }

    public void ShowScreen() => SetCanvasGroupAlpha(1);
    public void HideScreen() => SetCanvasGroupAlpha(0);

    public void EnableInteractability() => SetCanvasGroupState(true);
    public void DisableInteractability() => SetCanvasGroupState(false);

    private void SetCanvasGroupState(bool state) {
        CanvasGroupComponent.interactable = state;
        CanvasGroupComponent.blocksRaycasts = state;
    }

    private void SetCanvasGroupAlpha(float alpha) {
        CanvasGroupComponent.alpha = alpha;
        isShown = alpha == 1 ? true : false;
        CanvasComponent.enabled = isShown;
    }
}
