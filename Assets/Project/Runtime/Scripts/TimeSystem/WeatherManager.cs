using System;
using UnityEngine;
using TMPro;
using UnityUtilities;
using System.Collections.Generic;
using SaveSystem;
using Cysharp.Threading.Tasks;

namespace WorldSimulation {
    public class WeatherManager : Singleton<WeatherManager>, IDataHandler {
        Camera mainCamera;
        public WeatherData WeatherData;
        [SerializeField] WeatherSettings _defaultWeatherSettings;
        [SerializeField] WeatherSettings _currentWeatherSettings;
        [SerializeField] List<WeatherSettings> WeatherSettingsList;
        [SerializeField] Dictionary<WeatherType, WeatherSettings> WeatherSettingsDictionary;

        [SerializeField] TextMeshProUGUI currentWeatherText;

        [SerializeField] ParticleSystem rainEffectParticles;
        [SerializeField] Vector2 rainTransformOffset;
        [SerializeField, Range(0f, 5f)] float rainChanceStep = 0.1f;
        [SerializeField] bool forceRaining = false;
        [SerializeField] bool infiniteRaining = false;
        [SerializeField] float lightLerpSpeed;

        Color _currentWeatherColor;
        bool _isRaining = false;
        [SerializeField, Range(0f, 100f)] float _currentRainChance = 10f;
        ParticleSystem.MainModule _psMain;
        ParticleSystem.EmissionModule _psEmission;
        private bool _weatherInitialized = false;

        public WeatherSettings CurrentWeatherSettings { get => _currentWeatherSettings; set => _currentWeatherSettings = value; }
        public Color CurrentWeatherColor => _currentWeatherColor;
        public bool IsRaining { get => _isRaining; }
        [Range(0f, 1f)] public float CurrentRainChance { get => _currentRainChance; set => _currentRainChance = value; }
        
        CountdownTimer _rainTimer;
        public CountdownTimer GetRainTimer() => _rainTimer;
        float _currentRainTime = 0f;
        float _maxRainTime;
        float _rainTimeSeconds => (_currentWeatherSettings ? _currentWeatherSettings.baseRainTimeHours : 2) * WorldTimeConstants.SecondsInHour;
        public float CurrentRainTime => _currentRainTime;
        public float MaxRainTime => _maxRainTime;
        
        public float PercentOfRain => CalculatePercentOfRain(TimeSpan.FromSeconds(_rainTimer.GetTime()));

        public TimerManager WeatherTimerManager = new();

        public static event Action OnRainStart = delegate {};
        public static event Action OnRainEnd = delegate {};

        private void OnEnable() {
            TimeService.OnHourChange += CalculateRainChance;
            _rainTimer.OnTimerStart += StartRaining;
            _rainTimer.OnTimerStop += EndRaining;

            WorldGenerator.OnWorldCreated += InitializeWorldWeather;
            WorldGenerator.OnWorldCleared += () => _weatherInitialized = false;
        }

        private void OnDisable() {
            TimeService.OnHourChange -= CalculateRainChance;
            _rainTimer.OnTimerStart -= StartRaining;
            _rainTimer.OnTimerStop -= EndRaining;

            WeatherTimerManager.DisposeTimers();

            WorldGenerator.OnWorldCreated -= InitializeWorldWeather;
            WorldGenerator.OnWorldCleared -= () => _weatherInitialized = false;
        }

        protected override void Awake() {
            base.Awake();

            WeatherTimerManager = new();

            _rainTimer = new CountdownTimer(_rainTimeSeconds, ref WeatherTimerManager);
            _psMain = rainEffectParticles.main;
            _psEmission = rainEffectParticles.emission;

            WeatherSettingsDictionary = new Dictionary<WeatherType, WeatherSettings>();
            
            foreach (WeatherSettings settings in WeatherSettingsList) {
                WeatherSettingsDictionary.AddIfNotExists(settings.weatherType, settings);
            }

            _currentWeatherSettings = WeatherSettingsDictionary.GetValueOrDefault(WeatherType.Clear);
        }

        private void Start() {
            mainCamera = this.GetMainCamera();
        }

        private void Update() {
            rainEffectParticles.transform.position = mainCamera.transform.position.Add(x: rainTransformOffset.x, y: rainTransformOffset.y);

            if (WeatherData.IsNull() ) return;
            UpdateWeather();
        }

        public void InitializeWorldWeather() {
            _currentWeatherSettings = WeatherSettingsDictionary.GetValueOrDefault(WeatherData.WeatherType);
            _isRaining = WeatherData.IsRaining;
            _currentRainChance = WeatherData.RainChance;
            _currentRainTime = WeatherData.RainTime;

            if (_rainTimer.IsNull()) {
                _rainTimer = new CountdownTimer(_currentRainTime, ref WeatherTimerManager);
                _rainTimer.OnTimerStart += StartRaining;
                _rainTimer.OnTimerStop += EndRaining;
            }
            else
                _rainTimer.Reset(_currentRainTime);

            if (_isRaining)
                _rainTimer.Restart();

            _currentWeatherColor = _isRaining && _currentWeatherSettings.IsNotNull() ? _currentWeatherSettings.weatherColor : _defaultWeatherSettings.weatherColor;

            _weatherInitialized = true;
        }

        void UpdateWeather() {
            _currentWeatherColor = _currentWeatherColor.SetRGBA(
                Mathf.SmoothStep(_currentWeatherColor.r, _currentWeatherSettings.weatherColor.r, Time.deltaTime * lightLerpSpeed),
                Mathf.SmoothStep(_currentWeatherColor.g, _currentWeatherSettings.weatherColor.g, Time.deltaTime * lightLerpSpeed),
                Mathf.SmoothStep(_currentWeatherColor.b, _currentWeatherSettings.weatherColor.b, Time.deltaTime * lightLerpSpeed)
            );

            if (_isRaining) {
                if (!infiniteRaining) WeatherTimerManager.UpdateTimers(Time.deltaTime * TimeManager.Instance.CurrentTimeSettings.timeMultiplier);
                _psMain.simulationSpeed = _currentWeatherSettings.baseRainSpeed * _currentWeatherSettings.rainSpeedCurve.Evaluate(PercentOfRain);
                _psEmission.rateOverTime = _currentWeatherSettings.baseRainAmount * _currentWeatherSettings.rainAmountCurve.Evaluate(PercentOfRain);
            }
            else {
                _currentRainChance += (TimeManager.Instance.InGameDeltaTime * rainChanceStep);
            }

            currentWeatherText.text = $"{(currentWeatherText.IsNotNull() ? _currentWeatherSettings.weatherTypeName : "Clear")}";
        }

        public void StartRaining() {
            _isRaining = true;
            rainEffectParticles.EnableEmission(_isRaining);
            rainEffectParticles.Play(true);
            AudioManager.instance?.PlayAmbience(_currentWeatherSettings.audioClip);

            OnRainStart?.Invoke();
        }

        public void EndRaining() {
            _rainTimer.Pause();
            _isRaining = false;
            rainEffectParticles.EnableEmission(_isRaining);
            rainEffectParticles.Stop(true);

            forceRaining = false;

            OnRainEnd?.Invoke();

            _currentWeatherSettings = WeatherSettingsDictionary.GetValueOrDefault(WeatherType.Clear);
        }

        void CalculateRainChance() {
            if (_isRaining) return;

            int rainChance = UnityEngine.Random.Range(0, 101);

            if (_currentRainChance > rainChance) {
                CalculateRainParemeters();
                _currentRainChance /= _currentWeatherSettings.rainChanceCutoff;
                _rainTimer.Reset(_currentRainTime);
                _rainTimer.Restart();
            }

            _currentRainChance = _currentRainChance.Clamp(0f, 1f);
        }

        void CalculateRainParemeters() {
            _currentWeatherSettings = WeatherSettingsList.FindAll(ws => ws.weatherType != WeatherType.Clear).GetRandomElement();
            _maxRainTime = (_currentWeatherSettings.rainTimeRangeCurve.Evaluate(UnityEngine.Random.Range(0f, 1f)) * _currentRainChance * _rainTimeSeconds).Round();
            _currentRainTime = _maxRainTime;
        }

        float CalculatePercentOfRain(TimeSpan timeSpan) {
            return (float)timeSpan.TotalSeconds % WorldTimeConstants.SecondsInDay / WorldTimeConstants.SecondsInDay;
        }

        public void ToggleForceRaining() {
            forceRaining = forceRaining.Toggle();

            if (forceRaining && !_isRaining) {
                CalculateRainParemeters();
                _rainTimer.Reset(_currentRainTime);
                _rainTimer.Restart();
            }
            else if (!forceRaining && _isRaining) {
                _rainTimer.Stop();
            }
        }

        public void ToggleInfiniteRaining() => infiniteRaining = infiniteRaining.Toggle();

        #region Weather Serialization
        public async UniTask CreateData() {
            WeatherData = new WeatherData(
                WeatherType.Clear,
                false,
                0,
                0,
                0
            );

            DataPersistenceManager.Instance.WriteData(WeatherData);

            Debug.Log("Created Weather Data!");

            await UniTask.Yield();
        }

        public async UniTask SaveData() {
            if (WeatherData.IsNull()) {
                Debug.Log("Failed to save, Weather Data is null!");
                return;
            }

            WeatherData inputWeather = new WeatherData(
                _currentWeatherSettings.weatherType,
                _isRaining,
                _currentRainChance,
                _rainTimer.IsNotNull() ? _rainTimer.GetTime() : 0,
                _maxRainTime
            );

            DataPersistenceManager.Instance.WriteData(inputWeather);

            await UniTask.Yield();
        }

        public async UniTask LoadData() {
            if (DataPersistenceManager.Instance.CheckData(WeatherData)) {
                WeatherData = DataPersistenceManager.Instance.ReadData(WeatherData);
            }
            else {
                await CreateData();
            }

            _weatherInitialized = false;

            await UniTask.Yield();
        }

        public async UniTask DeleteData() {
            if (DataPersistenceManager.Instance.ClearData(WeatherData)) {
                Debug.Log($"Deleted Weather Data!");
            }

            _weatherInitialized = false;

            await UniTask.Yield();
        }
        #endregion
    }
}
