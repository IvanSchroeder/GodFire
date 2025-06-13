using System;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityUtilities;
using System.Collections.Generic;
using SaveSystem;
using Cysharp.Threading.Tasks;

namespace WorldSimulation {
    public class TimeManager : Singleton<TimeManager>, IDataHandler {
        TimeService timeService;
        Camera mainCamera;
        [SerializeField] Light2D _sunLight;
        [SerializeField] Light2D _moonLight;
        public TimeData TimeData;
        [SerializeField] TimeSettings _defaultTimeSettings;
        [SerializeField] TimeSettings _currentTimeSettings;
        [SerializeField] List<TimeSettings> TimeSettingsList;
        int timeSettingsIndex = 0;
        public TimeSettings CurrentTimeSettings { get => _currentTimeSettings; set => _currentTimeSettings = value; }
        [SerializeField] TimeFormat timeFormat = TimeFormat.FullHours;
        [SerializeField] bool _passTime = true;
        public bool PassTime { get => _passTime; set => _passTime = value; }
        [SerializeField] float _inGameDeltaTime;
        public float InGameDeltaTime { get => _inGameDeltaTime; }

        public int startDay = 0;
        [Range(0, 23)] public float startHour = 9;
        [Range(0, 23)] public float midnightHour = 0;
        [Range(0, 23)] public float sunriseHour = 6;
        [Range(0, 23)] public float noonHour = 12;
        [Range(0, 23)] public float eveningHour = 16;
        [Range(0, 23)] public float sunsetHour = 19;
        [Range(0, 23)] public float nightHour = 21;
        private bool _timeInitialized = false;

        [SerializeField] TextMeshProUGUI currentTimeText;
        [SerializeField] TextMeshProUGUI currentDayText;
        [SerializeField] TextMeshProUGUI currentDayPeriodText;

        [SerializeField] Gradient sunLightGradient;
        [SerializeField, Range (0f, 1f)] float _baseSunLightIntensity;
        [SerializeField] float _currentSunLightInsensity;
        [SerializeField] Gradient moonLightGradient;
        [SerializeField, Range (0f, 1f)] float _baseMoonLightIntensity;
        [SerializeField] float _currentMoonLightInsensity;
        [SerializeField] Gradient skyGradient;

        Color _currentSunLightColor;
        Color _currentMoonLightColor;
        [SerializeField] Color moonLightColor;

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

        void OnEnable() {
            TimeService.OnDayPeriodChange += UpdateDayPeriod;
            TimeService.OnSunrise += UpdateAmbience;
            TimeService.OnNight += UpdateAmbience;
            TimeService.OnNewDay += UpdateCurrentDay;

            WeatherManager.OnRainEnd += UpdateAmbience;

            WorldGenerator.OnWorldCreated += InitializeWorldTime;
            WorldGenerator.OnWorldCleared += () => _timeInitialized = false;
        }

        void OnDisable() {
            TimeService.OnDayPeriodChange -= UpdateDayPeriod;
            TimeService.OnSunrise -= UpdateAmbience;
            TimeService.OnNight -= UpdateAmbience;
            TimeService.OnNewDay -= UpdateCurrentDay;

            WeatherManager.OnRainEnd -= UpdateAmbience;

            WorldGenerator.OnWorldCreated -= InitializeWorldTime;
            WorldGenerator.OnWorldCleared -= () => _timeInitialized = false;
        }

        protected override void Awake() {
            base.Awake();
            
            ResetTimeSettings();
            timeService = new TimeService(startHour, startDay, midnightHour, sunriseHour, noonHour, eveningHour, sunsetHour, nightHour);
        }

        private void Start() {
            mainCamera = this.GetMainCamera();

            OnGameStart?.Invoke();
        }

        private void Update() {
            if (TimeData.IsNull() || !_timeInitialized) return;

            UpdateTimeOfDay();
        }

        public void InitializeWorldTime() {
            timeService.SetCurrentTime(TimeData.WorldTime);
            timeService.SetCurrentDay(TimeData.WorldDay);
            _timeInitialized = true;
        }

        void UpdateTimeOfDay() {
            if (_passTime)
                _inGameDeltaTime = _currentTimeSettings.timeMultiplier * Time.deltaTime;
            else
                _inGameDeltaTime = 0 * Time.deltaTime;

            timeService.UpdateTime(_inGameDeltaTime);

            if (currentTimeText.IsNotNull()) {
                currentTimeText.text = (timeFormat == TimeFormat.FullHours)
                ? timeService.CurrentTime.ToString("HH:mm:ss")
                : timeService.CurrentTime.ToString("hh:mm:ss") + (timeService.IsPastMidnight ? " AM" : " PM");
            }

            UpdateSunLightColor();
            UpdateMoonLightColor();

            // _sunLight.color = (_currentSunLightColor + _currentMoonLightColor) * WeatherManager.Instance.CurrentWeatherColor;
            _sunLight.color = _currentSunLightColor;

            mainCamera.backgroundColor = skyGradient.Evaluate(timeService.PercentOfDay) * WeatherManager.Instance.CurrentWeatherColor;
        }

        void UpdateSunLightColor() {
            _currentSunLightColor = sunLightGradient.Evaluate(timeService.PercentOfDay) * WeatherManager.Instance.CurrentWeatherColor;
            _currentSunLightInsensity = timeService.IsDayTime ? _baseSunLightIntensity : 0;

            // _sunLight.intensity = _currentSunLightInsensity;
        }

        void UpdateMoonLightColor() {
            _currentMoonLightColor = moonLightGradient.Evaluate(timeService.PercentOfDay) * moonLightColor;
            _moonLight.color = _currentMoonLightColor * WeatherManager.Instance.CurrentWeatherColor;

            // _currentMoonLightInsensity = timeService.IsNightTime ? _baseMoonLightIntensity : 0;
            _currentMoonLightInsensity = _baseMoonLightIntensity;
            _moonLight.intensity = _currentMoonLightInsensity;
        }

        void UpdateCurrentDay() {
            if (currentDayText.IsNotNull()) {
                currentDayText.text = $"Day {timeService.CurrentDay}";
            }
        }

        void UpdateDayPeriod() {
            if (currentDayPeriodText.IsNotNull()) {
                currentDayPeriodText.text = $"{timeService.DayPeriod}";
            }
        }

        void UpdateAmbience() {
            if (WeatherManager.Instance.IsRaining) return;

            if (timeService.IsDayTime)
                AudioManager.instance.PlayAmbience("ClearDay", false);
            else
                AudioManager.instance.PlayAmbience("Night", false);
        }

        public void NextTimeSettings() {
            if (TimeSettingsList.IsNullOrEmpty()) Debug.LogError("Time Settings List is empty!");

            timeSettingsIndex++;

            if (timeSettingsIndex >= TimeSettingsList.Count) {
                timeSettingsIndex = 0;
            }

            SetTimeSettings();
            SetPassTime(true);
        }

        public void PreviousTimeSettings() {
            if (TimeSettingsList.IsNullOrEmpty()) Debug.LogError("Time Settings List is empty!");

            timeSettingsIndex--;

            if (timeSettingsIndex <= 0) {
                timeSettingsIndex = TimeSettingsList.Count - 1;
            }

            SetTimeSettings();
            SetPassTime(true);
        }

        public void ResetTimeSettings() {
            if (TimeSettingsList.IsNullOrEmpty()) Debug.LogError("Time Settings List is empty!");

            timeSettingsIndex = TimeSettingsList.IndexOf(_defaultTimeSettings);

            SetTimeSettings();
            SetPassTime(true);
        }

        void SetTimeSettings() => _currentTimeSettings = TimeSettingsList?.GetElement(timeSettingsIndex);

        public void TogglePassTime() => SetPassTime(!PassTime);
        public void SetPassTime(bool state) => PassTime = state;

    #region Time Serialization
        public async UniTask CreateData() {
            TimeData = new TimeData(
                DateTime.Now.Date + TimeSpan.FromHours(startHour),
                startDay
            );

            DataPersistenceManager.Instance.WriteData(TimeData);

            Debug.Log("Created Time Data!");

            await UniTask.Yield();
        }

        public async UniTask SaveData() {
            if (TimeData.IsNull()) {
                Debug.Log("Failed to save, Time Data is null!");
                return;
            }

            TimeData inputTime = new TimeData(
                timeService.CurrentTime,
                timeService.CurrentDay
            );

            DataPersistenceManager.Instance.WriteData(inputTime);

            await UniTask.Yield();
        }

        public async UniTask LoadData() {
            if (DataPersistenceManager.Instance.CheckData(TimeData)) {
                TimeData = DataPersistenceManager.Instance.ReadData(TimeData);
            }
            else {
                await CreateData();
            }

            await UniTask.Yield();
        }

        public async UniTask DeleteData() {
            if (DataPersistenceManager.Instance.ClearData(TimeData)) {
                Debug.Log($"Deleted Time Data!");
            }
            else {
                Debug.Log($"Time Data doesnt exist!");
            }

            _timeInitialized = false;

            await UniTask.Yield();
        }
    #endregion
    }
}
