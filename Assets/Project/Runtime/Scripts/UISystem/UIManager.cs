using UnityEngine;
using UnityUtilities;
using TMPro;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class UIManager : Singleton<UIManager> {
    public SerializedDictionary<string, UIScreenController> UIScreensDictionary = new();
    public string startingScreen = "";
    public UIScreenController currentShownUIScreen;
    public GameObject blackScreenCanvas;
    private Image _blackScreenImage;
    private Material _blackScreenMaterial;

    public FadeType CurrentFadeType = FadeType.PlainBlack;

    public enum FadeType {
        PlainBlack,
        Shutters,
        RadialWipe,
        Goop
    }

    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;
    public Ease fadeInEase = Ease.Linear;
    public Ease fadeOutEase = Ease.Linear;

    private int FADE_AMOUNT = Shader.PropertyToID("_FadeAmount");

    private int USE_PLAIN_BLACK = Shader.PropertyToID("_UsePlainBlack");
    private int USE_SHUTTERS = Shader.PropertyToID("_UseShutters");
    private int USE_RADIAL_WIPE = Shader.PropertyToID("_UseRadialWipe");
    private int USE_GOOP = Shader.PropertyToID("_UseGoop");

    private int? _lastEffect;

    protected override void Awake() {
        base.Awake();

        _blackScreenImage = blackScreenCanvas.GetComponent<Image>();

        Material mat = _blackScreenImage.material;
        _blackScreenImage.material = new Material(mat);
        _blackScreenMaterial = _blackScreenImage.material;

        _lastEffect = USE_PLAIN_BLACK;
    }

    private async void Start() {
        await Init();
    }

    private async Task Init() {
        HideAllScreens();
        currentShownUIScreen = UIScreensDictionary.GetValueOrDefault(startingScreen);
        currentShownUIScreen.ShowScreen();
        currentShownUIScreen.DisableInteractability();

        await FadeIn(FadeType.PlainBlack);

        currentShownUIScreen.EnableInteractability();
    }

    public async UniTask FadeOut(FadeType fadeType, Action actionToDo = null) {
        ChangeFadeEffect(fadeType);
        await StartFadeOut();

        actionToDo?.Invoke();

        await UniTask.Yield();
    }

    public async UniTask FadeIn(FadeType fadeType, Action actionToDo = null) {
        ChangeFadeEffect(fadeType);
        await StartFadeIn();

        actionToDo?.Invoke();

        await UniTask.Yield();
    }

    public void ChangeScreens(string screenName) {
        ScreenFade(screenName).AsAsyncUnitUniTask();
    }

    private async UniTask ScreenFade(string screenName) {
        await FadeOut(FadeType.PlainBlack);
        
        if (currentShownUIScreen.IsNotNull() && currentShownUIScreen.IsShown) {
            currentShownUIScreen.HideScreen();
            currentShownUIScreen.DisableInteractability();
        }

        currentShownUIScreen = UIScreensDictionary.GetValueOrDefault(screenName);
        currentShownUIScreen.ShowScreen();

        await WaitFor.Delay(0.2f, true);

        await FadeIn(FadeType.PlainBlack);

        currentShownUIScreen.EnableInteractability();

        await UniTask.Yield();
    }

    private void HideAllScreens(UIScreenController exceptionScreen = null) {
        foreach (UIScreenController screen in UIScreensDictionary.Values) {
            if (exceptionScreen.IsNotNull() && screen == exceptionScreen) continue;

            screen?.HideScreen();
        }
    }

    private void ShowBlackScreen() => _blackScreenMaterial.SetFloat(FADE_AMOUNT, 1f);
    private void HideBlackScreen() => _blackScreenMaterial.SetFloat(FADE_AMOUNT, 0f);

    private async UniTask StartFadeOut() {
        HideBlackScreen();
        await _blackScreenMaterial.DOFloat(1f, FADE_AMOUNT, fadeOutDuration).SetEase(fadeOutEase).IsComplete();

        DebugFade("Fade Out Completed!");

        await UniTask.Yield();
    }

    private async UniTask StartFadeIn() {
        ShowBlackScreen();
        await _blackScreenMaterial.DOFloat(0f, FADE_AMOUNT, fadeInDuration).SetEase(fadeInEase).IsComplete();

        DebugFade("Fade In Completed!");

        await UniTask.Yield();
    }

    private void ChangeFadeEffect(FadeType fadeType) {
        if (_lastEffect.HasValue) _blackScreenMaterial.SetFloat(_lastEffect.Value, 0f);

        switch (fadeType) {
            case FadeType.PlainBlack:
                SwitchEffect(USE_PLAIN_BLACK);
                break;
            case FadeType.Shutters:
                SwitchEffect(USE_SHUTTERS);
                break;
            case FadeType.RadialWipe:
                SwitchEffect(USE_RADIAL_WIPE);
                break;
            case FadeType.Goop:
                SwitchEffect(USE_GOOP);
                break;
        }
    }

    private void SwitchEffect(int effectToTurnOn) {
        _blackScreenMaterial.SetFloat(effectToTurnOn, 1f);

        _lastEffect = effectToTurnOn;
    }

    void DebugFade(string message) {
        Debug.Log(message);
    }
}
