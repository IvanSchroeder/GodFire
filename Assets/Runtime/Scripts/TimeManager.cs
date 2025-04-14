using System;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;
using ExtensionMethods;
using Utilities;
using UnityEngine.UI;
using System.Collections.Generic;

namespace WorldTime {
    [RequireComponent(typeof(Light2D))]
    public class TimeManager : MonoBehaviour {
        public static TimeManager instance;

        Camera mainCamera;
        TimeService service;
        Light2D _light;
        [SerializeField] public TimeSettings timeSettings;
        [SerializeField] TimeFormat timeFormat = TimeFormat.FullHours;
        [SerializeField] List<RainSettings> RainSettingsList;
        [SerializeField] public RainSettings currentRainSettings;

        [SerializeField] private Gradient lightGradient;
        [SerializeField] private Gradient skyGradient;
        [SerializeField] private Color clearColor;
        [SerializeField] private Color currentLightColor;
        [SerializeField] private Color currentWeatherColor;
        [SerializeField] private float lerpSpeed;
        [SerializeField] ParticleSystem rainEffectParticles;
        ParticleSystem.MainModule psMain;
        ParticleSystem.EmissionModule psEmission;

        [SerializeField] TextMeshProUGUI currentTimeText;
        [SerializeField] TextMeshProUGUI currentDayText;
        [SerializeField] TextMeshProUGUI currentDayPeriodText;
        [SerializeField] TextMeshProUGUI currentWeatherText;
        [SerializeField] TextMeshProUGUI currentDayPercentageText;
        [SerializeField] TextMeshProUGUI currentRainPercentageText;

        [SerializeField] bool startsRaining = false;
        public bool isRaining = false;
        float currentRainTime = 0f;
        [SerializeField, Range(1.1f, 5f)] float rainChanceStep = 1.1f;
        [SerializeField, Range(0f, 1f)] float currentRainChance = 0.1f;
        
        float rainTimeSeconds => currentRainSettings.baseRainTimeHours * WorldTimeConstants.SecondsInHour;
        CountdownTimer rainTimer;

        public static event Action OnSunrise {
            add => TimeService.OnSunrise += value;
            remove => TimeService.OnSunrise -= value;
        }

        public static event Action OnNoon {
            add => TimeService.OnNoon += value;
            remove => TimeService.OnNoon -= value;
        }
        public static event Action OnEvening {
            add => TimeService.OnEvening += value;
            remove => TimeService.OnEvening -= value;
        }

        public static event Action OnSunset {
            add => TimeService.OnSunset += value;
            remove => TimeService.OnSunset -= value;
        }

        public static event Action OnNight {
            add => TimeService.OnNight += value;
            remove => TimeService.OnNight -= value;
        }
        public static event Action OnMidnight {
            add => TimeService.OnMidnight += value;
            remove => TimeService.OnMidnight -= value;

        }

        public static event Action OnHourChange {
            add => TimeService.OnHourChange += value;
            remove => TimeService.OnHourChange -= value;
        }

        public static event Action OnNewDay {
            add => TimeService.OnNewDay += value;
            remove => TimeService.OnNewDay -= value;
        }

        public static event Action OnDayPeriodChange {
            add => TimeService.OnDayPeriodChange += value;
            remove => TimeService.OnDayPeriodChange -= value;
        }

        public static event Action OnGameStart = delegate {};
        public static event Action OnRainStart = delegate {};
        public static event Action OnRainEnd = delegate {};

        void OnEnable() {
            TimeService.OnDayPeriodChange += UpdateDayPeriod;
            TimeService.OnDayPeriodChange += CalculateRainChance;
            TimeService.OnSunrise += UpdateAmbience;
            TimeService.OnNight += UpdateAmbience;
            TimeService.OnNewDay += UpdateCurrentDay;

            rainTimer.OnTimerStart += StartRaining;
            rainTimer.OnTimerStop += EndRaining;

            OnRainStart += UpdateWeather;
            OnRainEnd += UpdateWeather;
        }

        void OnDisable() {
            TimeService.OnDayPeriodChange -= UpdateDayPeriod;
            TimeService.OnDayPeriodChange -= CalculateRainChance;
            TimeService.OnSunrise -= UpdateAmbience;
            TimeService.OnNight -= UpdateAmbience;
            TimeService.OnNewDay -= UpdateCurrentDay;

            rainTimer.OnTimerStart -= StartRaining;
            rainTimer.OnTimerStop -= EndRaining;

            OnRainStart -= UpdateWeather;
            OnRainEnd -= UpdateWeather;
        }

        void Awake() {
            if (instance == null) {
                instance = this;
            }
            else if (instance != this) {
                Destroy(gameObject);
            }
            service = new TimeService(timeSettings);
            _light = GetComponent<Light2D>();
            currentRainSettings = RainSettingsList.GetRandomElement();
            rainTimer = new CountdownTimer(rainTimeSeconds);
            psMain = rainEffectParticles.main;
            psEmission = rainEffectParticles.emission;
        }

        void Start() {
            mainCamera = this.GetMainCamera();

            if (startsRaining) rainTimer.Restart();
            else EndRaining();

            OnGameStart?.Invoke();
        }

        void Update() {
            UpdateTimeOfDay();
        }

        void UpdateTimeOfDay() {
            service.UpdateTime(Time.deltaTime * timeSettings.timeMultiplier);

            if (currentTimeText != null) {
                currentTimeText.text = (timeFormat == TimeFormat.FullHours)
                ? service.CurrentTime.ToString("HH:mm:ss")
                : service.CurrentTime.ToString("hh:mm:ss") + (service.IsMidnight ? " AM" : " PM");
            }

            if (currentDayPercentageText != null) {
                currentDayPercentageText.text = $"{service.PercentOfDay:f2}";
            }

            if (currentRainPercentageText != null) {
                currentRainPercentageText.text = $"{PercentOfRain:f2}";
            }

            if (!isRaining) {
                // currentWeatherColor = Color.Lerp(currentWeatherColor, clearColor, Time.deltaTime);
                currentWeatherColor.r = Mathf.SmoothStep(currentWeatherColor.r, clearColor.r, Time.deltaTime * lerpSpeed);
                currentWeatherColor.g = Mathf.SmoothStep(currentWeatherColor.g, clearColor.g, Time.deltaTime * lerpSpeed);
                currentWeatherColor.b = Mathf.SmoothStep(currentWeatherColor.b, clearColor.b, Time.deltaTime * lerpSpeed);
            }
            else {
                // currentWeatherColor = Color.Lerp(currentWeatherColor, rainSettings.weatherColor, Time.deltaTime);
                rainTimer.Tick(Time.deltaTime * timeSettings.timeMultiplier);
                SetRainingParameters();
                currentWeatherColor.r = Mathf.SmoothStep(currentWeatherColor.r, currentRainSettings.weatherColor.r, Time.deltaTime * lerpSpeed);
                currentWeatherColor.g = Mathf.SmoothStep(currentWeatherColor.g, currentRainSettings.weatherColor.g, Time.deltaTime * lerpSpeed);
                currentWeatherColor.b = Mathf.SmoothStep(currentWeatherColor.b, currentRainSettings.weatherColor.b, Time.deltaTime * lerpSpeed);
            }

            currentLightColor = lightGradient.Evaluate(service.PercentOfDay);
            _light.color = currentLightColor * currentWeatherColor;
            
            mainCamera.backgroundColor = skyGradient.Evaluate(service.PercentOfDay) * currentWeatherColor;
        }

        void UpdateCurrentDay() {
            if (currentDayText != null) {
                currentDayText.text = $"Day {service.CurrentDay}";
            }
        }

        void UpdateDayPeriod() {
            if (currentDayPeriodText != null) {
                currentDayPeriodText.text = $"{service.DayPeriod}";
            }
        }

        void UpdateWeather() {
            if (currentWeatherText != null) {
                currentWeatherText.text = $"{(isRaining ? currentRainSettings.rainTypeName : "Clear")}";
            }
        }

        void UpdateAmbience() {
            if (isRaining) return;

            if (service.IsDay)
                AudioManager.instance.PlayAmbience("ClearDay", false);
            else
                AudioManager.instance.PlayAmbience("Night", false);
        }

        void SetRainingParameters() {
            psMain.simulationSpeed = currentRainSettings.baseRainSpeed * currentRainSettings.rainSpeedCurve.Evaluate(PercentOfRain);
            psEmission.rateOverTime = currentRainSettings.baseRainAmount * currentRainSettings.rainAmountCurve.Evaluate(PercentOfRain);
        }

        void StartRaining() {
            currentRainSettings = RainSettingsList.GetRandomElement();
            isRaining = true;
            rainEffectParticles.EnableEmission(isRaining);
            rainEffectParticles.Play(true);
            AudioManager.instance?.PlayAmbience(currentRainSettings.audioClip);

            OnRainStart?.Invoke();

            // Debug.Log(currentRainSettings.rainStartLogMessage);
        }

        void EndRaining() {
            rainTimer.Pause();
            isRaining = false;
            rainEffectParticles.EnableEmission(isRaining);
            rainEffectParticles.Stop(true);
            
            if (service.IsDay)
                AudioManager.instance.PlayAmbience("ClearDay", false);
            else
                AudioManager.instance.PlayAmbience("Night", false);

            OnRainEnd?.Invoke();

            // Debug.Log(currentRainSettings.rainStopLogMessage);
            currentRainSettings = RainSettingsList.GetRandomElement();
        }

        void CalculateRainChance() {
            if (isRaining) return;

            float rainChance = UnityEngine.Random.Range(0f, 1f);

            if (currentRainChance >= rainChance) {
                currentRainTime = (currentRainSettings.rainTimeRangeCurve.Evaluate(UnityEngine.Random.Range(0f, 1f)) * rainTimeSeconds).Round();
                rainTimer.Reset(currentRainTime);
                rainTimer.Restart();
                currentRainChance /= currentRainSettings.rainChanceCutoff;
            }
            else {
                currentRainChance += currentRainChance * rainChanceStep;
            }

            currentRainChance = currentRainChance.Clamp(0f, 1f);
        }

        float PercentOfRain => CalculatePercentOfRain(TimeSpan.FromSeconds(rainTimer.GetTime()));

        float CalculatePercentOfRain(TimeSpan timeSpan) {
            return (float)timeSpan.TotalSeconds % WorldTimeConstants.SecondsInDay / WorldTimeConstants.SecondsInDay;
        }
    }
}
