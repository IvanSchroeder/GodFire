using UnityEngine;
using UnityUtilities;
using DG.Tweening;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
public class UIScreenController : MonoBehaviour {
    public string ScreenName = "";
    private Canvas ScreenCanvas;
    private CanvasGroup ScreenCanvasGroup;
    public bool IsShown = false;

    void Awake() {
        ScreenCanvas = GetComponent<Canvas>();
        ScreenCanvasGroup = GetComponent<CanvasGroup>();
    }

    public void ShowScreen() => SetCanvasGroupAlpha(1);
    public void HideScreen() => SetCanvasGroupAlpha(0);

    public void EnableInteractability() => SetCanvasGroupState(true);
    public void DisableInteractability() => SetCanvasGroupState(false);

    private void SetCanvasGroupState(bool state) {
        ScreenCanvasGroup.interactable = state;
        ScreenCanvasGroup.blocksRaycasts = state;
    }

    private void SetCanvasGroupAlpha(float alpha) {
        ScreenCanvasGroup.alpha = alpha;
        IsShown = alpha == 1 ? true : false;
        ScreenCanvas.enabled = IsShown;
    }

    // private async UniTask FadeCanvasGroup(float startingAlpha, float targetAlpha, float duration, bool ignoreTimeScale = false) {
    //     float initialTime = 0;

    //     while (initialTime < duration) {
    //         SetCanvasGroupAlpha()

    //         initialTime += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
    //     }

    //     await UniTask.Yield();
    // }
}
