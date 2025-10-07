using System.Collections.Generic;
using UnityUtilities;
using UnityEngine;
using WorldSimulation;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using System;
using UnityEngine.EventSystems;
using UnityEditor;

[Serializable]
public class CampfireData : WorldObjectData {
    public float Health = 100;
    public float Fuel = 100;
    public bool IsLit = false;
    public bool IsOutOfFuel = false;
    public bool IsBurnt = false;
}

public class CampfireObject : WorldObject, IInteractable, IBurner, IDestroyable {
    [Header("References")]
    [Space(2)]
    [SerializeField] Animator Anim;
    [SerializeField] AudioSource AmbienceSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] ParticleSystem FireEffect;
    [SerializeField] ParticleSystem.EmissionModule fireEmission;
    [SerializeField] ParticleSystem SparkEffect;
    [SerializeField] ParticleSystem SmokeEffect;
    [SerializeField] Light2D MainLight;
    [SerializeField] Canvas worldCanvas;
    [SerializeField] Slider healthSlider;
    [SerializeField] Image healthFillImage;
    [SerializeField] Slider fuelSlider;
    [SerializeField] Image fuelFillImage;

    CampfireData LocalCampfireData { get => GetObjectData<CampfireData>(); set => SetObjectData(value); }

#region Interactable
    [field: SerializeField] public InteractableTrigger InteractableTrigger { get; set ; }
#endregion

#region Burner
    [Space(5)]
    [Header("Burner Settings")]
    [Space(2)]
    [SerializeField] bool consumeFuel = true;
    [SerializeField] float baseFuelConsumptionRate;
    [SerializeField] int baseFireAmount;
    [SerializeField] AnimationCurve fireAmountCurve;
    [SerializeField] float lightIntensity;
    [SerializeField] float lightOuterRadius;
    [SerializeField] Gradient healthColorGradient;
    [SerializeField] Gradient fuelLitColorGradient;
    [SerializeField] Gradient fuelUnlitColorGradient;
    [field: SerializeField] public BurnerTrigger BurnerTrigger { get; set; }

    public List<Item> BurningItemsList;
    public Queue<Item> BurningItemsQueue;
    [field: SerializeField] public float MaxFuel { get; set; } = 100;
#endregion

#region Destroyable
    [field: SerializeField] public float MaxHealth { get; set; } = 100;
    #endregion

    public static readonly int LitHash = Animator.StringToHash("Lit");
    public static readonly int UnlitHash = Animator.StringToHash("Unlit");
    public static readonly int BurntHash = Animator.StringToHash("Burnt");

#if UNITY_EDITOR
    protected override void OnValidate() {
        base.OnValidate();

        if (LocalCampfireData.IsNull()) LocalCampfireData = new CampfireData();
        LocalCampfireData.Name = "Campfire";

        UpdateHealth(MaxHealth);
    }
#endif

    protected override void Awake() {
        base.Awake();
        
        fireEmission = FireEffect.emission;

        if (BurnerTrigger.IsNull()) BurnerTrigger = this.GetComponentInHierarchy<BurnerTrigger>();
        BurnerTrigger?.Init();
    }

    void Start() {
        worldCanvas.worldCamera = this.GetMainCamera();

        BurningItemsList = new List<Item>();
    }

    public override void Init() {
        LocalCampfireData = GetObjectData<CampfireData>();
        UpdateHealth(LocalCampfireData.Health);
        UpdateFuel(LocalCampfireData.Fuel);

        if (LocalCampfireData.IsBurnt) {
            BurnCampfire();
        }
        else {
            if (LocalCampfireData.IsLit) {
                LitFire();
            }
            else {
                UnlitFire();
            }
        }

        InteractableTrigger?.Init();

        _ = SpriteFlash(ShaderDataSO.GetSpriteFlashSettings("SPAWN"));
    }

    void Update() {
        ConsumeFuel();
        UpdateHealthInfo();
        UpdateFuelInfo();
    }

    public void OnDestroyed() {
        BurnCampfire();
    }

    public void OnInteract() {
        CheckCampfireStatus();
    }

    public void OnHoverStart() {
        _ = SpriteFlash(ShaderDataSO.GetSpriteFlashSettings("HOVER"));
    }

    public void OnHoverEnd() {}

    public void OnItemEntered<T>(Item item) where T : IBurnable {
        if (!LocalCampfireData.IsBurnt) {
            if (LocalCampfireData.IsLit) {
                AddFuel(item.GetFuelAmount());

                if (item.BurnsInstantly) {
                    item.CompleteBurning();
                    BurstSparks();
                    item.DespawnObject();
                    _ = SpriteFlash(ShaderDataSO.GetSpriteFlashSettings("BURN"));
                }
                else
                    BurningItemsList.Add(item);
            }
            else {
                item.DespawnObject();
            }
        }
        else {
            UpdateHealth(LocalCampfireData.Health + item.GetFuelAmount());
            _ = SpriteFlash(ShaderDataSO.GetSpriteFlashSettings("UNLIT"));
            item.DespawnObject();
        }
    }

    public void OnItemBurn() {
    }

    public void OnItemExited<T>(Item item) where T : IBurnable {
        item.EndBurn();
        BurningItemsList.Remove(item);
    }
 
    public void UpdateHealth(float healthAmount) {
        LocalCampfireData.Health = healthAmount.Clamp(0, MaxHealth).ToIntFloor();

        if (LocalCampfireData.IsBurnt && LocalCampfireData.Health >= MaxHealth) {
            RestoreCampfire();
        }
    }

    void AddFuel(float fuelAmount) {
        UpdateFuel(LocalCampfireData.Fuel + fuelAmount);
    }

    void UpdateFuel(float fuelAmount) {
        LocalCampfireData.Fuel = fuelAmount;
        LocalCampfireData.Fuel = LocalCampfireData.Fuel.Clamp(0, MaxFuel);
    }

    void ConsumeFuel() {
        if (LocalCampfireData.IsBurnt || !LocalCampfireData.IsLit || !consumeFuel) return;
        
        if (!LocalCampfireData.IsOutOfFuel) {
            AddFuel(-(baseFuelConsumptionRate
                * (WeatherManager.Instance.IsRaining && WeatherManager.Instance.CurrentWeatherSettings.IsNotNull() ? WeatherManager.Instance.CurrentWeatherSettings.fuelConsuptionMultiplier : 1)
                * (TimeManager.Instance.CurrentTimeSettings.IsNotNull() ? TimeManager.Instance.InGameDeltaTime : Time.deltaTime)
            ));
        }
        else {
            OnDestroyed();
            BurnCampfire();
        }
    }

    public void CheckCampfireStatus() {
        if (LocalCampfireData.IsBurnt) return;

        if (!LocalCampfireData.IsLit) {
            LitFire();
        }
        else if (LocalCampfireData.IsLit) {
            UnlitFire();
        }
    }

    void LitFire() {
        LocalCampfireData.IsLit = true;

        FireEffect.Play();
        SmokeEffect.Stop();
        Anim.CrossFade(LitHash, 0f);
        _ = SpriteFlash(ShaderDataSO.GetSpriteFlashSettings("LIT"));

        fuelSlider.gameObject.SetActive(true);
        healthSlider.gameObject.SetActive(false);

        BurstSparks();
        AmbienceSource.Play();

        BurnerTrigger.SetCombustibility(true);
    }

    void UnlitFire() {
        LocalCampfireData.IsLit = false;

        FireEffect.Stop();
        SmokeEffect.Play();
        Anim.CrossFade(UnlitHash, 0f);
        _ = SpriteFlash(ShaderDataSO.GetSpriteFlashSettings("UNLIT"));

        fuelSlider.gameObject.SetActive(true);
        healthSlider.gameObject.SetActive(false);

        AmbienceSource.Stop();

        BurnerTrigger.SetCombustibility(false);
    }

    void BurnCampfire(bool burst = true) {
        LocalCampfireData.IsLit = false;
        LocalCampfireData.IsBurnt = true;

        FireEffect.Stop();
        SmokeEffect.Play();
        if (burst) BurstSparks();
        Anim.CrossFade(BurntHash, 0f);
        _ = SpriteFlash(ShaderDataSO.GetSpriteFlashSettings("BURNOUT"));

        fuelSlider.gameObject.SetActive(false);
        healthSlider.gameObject.SetActive(true);

        UpdateHealth(0f);
        
        AmbienceSource.Stop();

        BurnerTrigger.SetCombustibility(true);
    }

    void RestoreCampfire() {
        LocalCampfireData.IsBurnt = false;
        UpdateFuel(MaxFuel * 0.1f);
        UnlitFire();
    }

    void UpdateFuelInfo() {
        float fuelPercentage = CalculateFuelPercentage();
        fireEmission.rateOverTime = (fireAmountCurve.Evaluate(fuelPercentage) * baseFireAmount).ToIntRound();
        MainLight.pointLightOuterRadius = lightOuterRadius;
        MainLight.pointLightInnerRadius = (fireAmountCurve.Evaluate(fuelPercentage) * (lightOuterRadius / 2)).Clamp(0, lightOuterRadius);
        MainLight.intensity = lightIntensity;
        fuelSlider.value = fuelPercentage;
        fuelFillImage.color = LocalCampfireData.IsLit ? fuelLitColorGradient.Evaluate(fuelPercentage) : fuelUnlitColorGradient.Evaluate(fuelPercentage);
    }

    void UpdateHealthInfo() {
        if (LocalCampfireData.Fuel > 0)
            LocalCampfireData.IsOutOfFuel = false;
        else
            LocalCampfireData.IsOutOfFuel = true;

        float healthPercentage = CalculateHealthPercentage();
        healthSlider.value = healthPercentage;
        healthFillImage.color = healthColorGradient.Evaluate(healthPercentage);
    }

    void BurstSparks() {
        SFXSource.PlayOneShot(AudioManager.Instance.GetSFXClip("FireCrack", true));
        SparkEffect.Play();
    }

    float CalculateHealthPercentage() {
        return LocalCampfireData.Health % (MaxHealth + 0.001f) / (MaxHealth + 0.001f);
    }

    float CalculateFuelPercentage() {
        return LocalCampfireData.Fuel % (MaxFuel + 0.001f) / (MaxFuel + 0.001f);
    }
}
