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
using SaveSystem;
using WorldSimulation;

public class UIManager : Singleton<UIManager> {
    [Header("References")]
    [Space(2)]
    public SerializedDictionary<string, UIScreenController> UIScreensDictionary = new();
    public string startingScreen = "";
    public UIScreenController currentShownUIScreen;

    [Space(5)]
    [Header("Animation Settings")]
    [Space(2)]
    public SerializedDictionary<string, SquashAndStretchSettings> SquashStretchSettingsDictionary = new();

    [Space(5)]
    [Header("Fade Screen Settings")]
    [Space(2)]
    public FadeType CurrentFadeType = FadeType.PlainBlack;
    public GameObject fadeScreenCanvas;
    private Image _fadeScreenImage;
    private Material _fadeScreenMaterial;
    public float fadeInDuration = 1f;
    public float fadeOutDuration = 1f;
    public float screenChangeDelay = 0.25f;
    public float sceneChangeDelay = 1f;
    public Ease fadeInEase = Ease.Linear;
    public Ease fadeOutEase = Ease.Linear;

    public enum FadeType {
        PlainBlack,
        Shutters,
        RadialWipe,
        Goop
    }

    private int FADE_AMOUNT = Shader.PropertyToID("_FadeAmount");
    private int USE_PLAIN_BLACK = Shader.PropertyToID("_UsePlainBlack");
    private int USE_SHUTTERS = Shader.PropertyToID("_UseShutters");
    private int USE_RADIAL_WIPE = Shader.PropertyToID("_UseRadialWipe");
    private int USE_GOOP = Shader.PropertyToID("_UseGoop");

    private int? _lastEffect;

    protected override void Awake() {
        base.Awake();

        _fadeScreenImage = fadeScreenCanvas.GetComponent<Image>();

        Material mat = _fadeScreenImage.material;
        _fadeScreenImage.material = new Material(mat);
        _fadeScreenMaterial = _fadeScreenImage.material;

        _lastEffect = USE_PLAIN_BLACK;
    }

    public async Task Init() {
        HideAllScreens();
        currentShownUIScreen = UIScreensDictionary.GetValueOrDefault(startingScreen);
        currentShownUIScreen.ShowScreen();
        currentShownUIScreen.DisableInteractability();


        await FadeIn(FadeType.PlainBlack);

        currentShownUIScreen.EnableInteractability();
    }

    private async UniTask FadeOut(FadeType fadeType, Action actionToDo = null) {
        ChangeFadeEffect(fadeType);
        await StartFadeOut();

        actionToDo?.Invoke();

        await UniTask.Yield();
    }

    private async UniTask FadeIn(FadeType fadeType, Action actionToDo = null) {
        ChangeFadeEffect(fadeType);
        await StartFadeIn();

        actionToDo?.Invoke();

        await UniTask.Yield();
    }

    public async void ChangeScreens(string screenName) {
        await ScreenFade(screenName);
    }

    public async void DeleteWorldData(string profileId) {
        await DataPersistenceManager.Instance.DeleteGameData(profileId);
        
        await UniTask.Yield();
    }

    public async void ToGameplayScene(string profileId) {
        await FadeOut(FadeType.PlainBlack);

        if (currentShownUIScreen.IsNotNull() && currentShownUIScreen.IsShown) {
            currentShownUIScreen.HideScreen();
            currentShownUIScreen.DisableInteractability();
        }

        await DataPersistenceManager.Instance.LoadGameData(profileId);

        currentShownUIScreen = UIScreensDictionary.GetValueOrDefault("HUD");
        currentShownUIScreen.ShowScreen();
        currentShownUIScreen.EnableInteractability();

        TimeManager.Instance.SetPassTime(false);

        await WaitFor.Delay(sceneChangeDelay, true);

        await FadeIn(FadeType.PlainBlack);

        await WaitFor.Delay(1f, true);
        TimeManager.Instance.SetPassTime(true);

        Debug.Log($"Loaded {profileId} in Gameplay Scene!");

        await UniTask.Yield();
    }

    public async void SaveGameData() {
        await FadeOut(FadeType.PlainBlack);

        currentShownUIScreen.DisableInteractability();

        await DataPersistenceManager.Instance.SaveGameData();

        await FadeIn(FadeType.PlainBlack);

        currentShownUIScreen.EnableInteractability();
        Debug.Log("Saved Game Data!");

        await UniTask.Yield();
    }

    public async void ToMainMenu() {
        await FadeOut(FadeType.PlainBlack);
        
        await HideCurrentScreen();

        await DataPersistenceManager.Instance.SaveGameData();

        await WorldGenerator.Instance.ClearGameplayScene();
        WeatherManager.Instance.EndRaining();
        AudioManager.Instance.StopAmbience();

        currentShownUIScreen = UIScreensDictionary.GetValueOrDefault("MAIN_MENU");
        currentShownUIScreen.ShowScreen();
        currentShownUIScreen.EnableInteractability();

        await WaitFor.Delay(sceneChangeDelay, true);

        await FadeIn(FadeType.PlainBlack);

        Debug.Log("Loaded Main Menu Screen!");

        await UniTask.Yield();
    }

    private async UniTask ScreenFade(string screenName) {
        await FadeOut(FadeType.PlainBlack);
        
        await HideCurrentScreen();

        currentShownUIScreen = UIScreensDictionary.GetValueOrDefault(screenName);
        currentShownUIScreen.ShowScreen();

        await WaitFor.Delay(screenChangeDelay, true);

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


    private async UniTask HideCurrentScreen() {
        if (currentShownUIScreen.IsNotNull() && currentShownUIScreen.IsShown) {
            currentShownUIScreen.HideScreen();
            currentShownUIScreen.DisableInteractability();
        }

        await UniTask.Yield();
    }
    
    private async UniTask StartFadeOut() {
        HideBlackScreen();

        await _fadeScreenMaterial.DOFloat(1f, FADE_AMOUNT, fadeOutDuration).SetEase(fadeOutEase).IsComplete();

        await UniTask.Yield();
    }

    private async UniTask StartFadeIn() {
        ShowFadeScreen();

        await _fadeScreenMaterial.DOFloat(0f, FADE_AMOUNT, fadeInDuration).SetEase(fadeInEase).IsComplete();

        await UniTask.Yield();
    }

    private void ShowFadeScreen() => _fadeScreenMaterial.SetFloat(FADE_AMOUNT, 1f);
    private void HideBlackScreen() => _fadeScreenMaterial.SetFloat(FADE_AMOUNT, 0f);

    private void ChangeFadeEffect(FadeType fadeType) {
        if (_lastEffect.HasValue) _fadeScreenMaterial.SetFloat(_lastEffect.Value, 0f);

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
        _fadeScreenMaterial.SetFloat(effectToTurnOn, 1f);

        _lastEffect = effectToTurnOn;
    }
}
