using System.Collections.Generic;
using UnityUtilities;
using UnityEngine;
using WorldSimulation;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class CampfireObject : WorldObject, IInteractable {
    [Header("References")]
    [Space(5f)]
    [SerializeField] Animator Anim;
    [SerializeField] AudioSource AmbienceSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] ParticleSystem FireEffect;
    [SerializeField] ParticleSystem.EmissionModule fireEmission;
    [SerializeField] ParticleSystem SparkEffect;
    [SerializeField] ParticleSystem SmokeEffect;
    [SerializeField] Collider2D BurnCollider;
    [SerializeField] Light2D MainLight;
    [SerializeField] Canvas worldCanvas;
    [SerializeField] Slider healthSlider;
    [SerializeField] Image healthFillImage;
    [SerializeField] Slider fuelSlider;
    [SerializeField] Image fuelFillImage;

    [field: SerializeField] public Collider2D InteractionCollider { get; set; }
    [field: SerializeField] public bool IsInteractable { get; set; } = true;
    [field: SerializeField] public bool HasInteractabilityCooldown { get; set; } = false;
    [field: SerializeField] public CountdownTimer InteractabilityTimer { get; set; }
    [field: SerializeField] public float InteractabilityCooldownSeconds { get; set; }

    [Space(10f)]
    [Header("Parameters")]
    [Space(5f)]

    [SerializeField] bool startsLit;
    [SerializeField] bool startsBurnt;
    [SerializeField] bool consumeFuel = true;
    [SerializeField] float maxHealth;
    [SerializeField] float maxFuelAmount;
    [SerializeField] float baseFuelConsumptionRate;
    [SerializeField] int baseFireAmount;
    [SerializeField] AnimationCurve fireAmountCurve;
    [SerializeField] float lightIntensity;
    [SerializeField] float lightOuterRadius;
    [SerializeField] Gradient healthColorGradient;
    [SerializeField] Gradient fuelLitColorGradient;
    [SerializeField] Gradient fuelUnlitColorGradient;

    [SerializeField] float currentHealth;
    [SerializeField] float currentFuelAmount;
    [SerializeField] bool isLit;
    [SerializeField] bool isBurnt;
    [SerializeField] bool isOutOfFuel;

    public List<Item> BurningItemsList;
    public Queue<Item> BurningItemsQueue;

    public readonly int LitHash = Animator.StringToHash("Lit");
    public readonly int UnlitHash = Animator.StringToHash("Unlit");
    public readonly int BurntHash = Animator.StringToHash("Burnt");


    void OnValidate() {
        if (worldObjectData.IsNull()) SetObjectData(new CampfireData());
        worldObjectData.Name = "Campfire";
    }

    public override void Awake() {
        SetObjectData(new CampfireData());
        worldObjectData.Name = "Campfire";
        fireEmission = FireEffect.emission;
    }

    void Start() {
        worldCanvas.worldCamera = this.GetMainCamera();

        BurningItemsList = new List<Item>();
    }

    public override void Init() {
        if (startsLit) {
            SetHealth(maxHealth);
            SetFuel(maxFuelAmount);
            LitFire();
        }
        else {
            if (startsBurnt) {
                SetFuel(0);
                BurnCampfire(false);
            }
            else {
                SetHealth(maxHealth);
                SetFuel(maxFuelAmount);
                UnlitFire();
            }
        }
    }

    void Update() {
        ConsumeFuel();
        UpdateHealthParameters();
        UpdateFuelParameters();
    }

    void OnTriggerEnter2D(Collider2D collider) {
        Item burnable = (Item)collider.gameObject.GetComponentInHierarchy<IBurnable>();

        if (burnable != null) {
            if (!isBurnt) {
                AddFuel(burnable.GetFuelAmount());
                if (isLit) {
                    if (burnable.BurnsInstantly) {
                        burnable.CompleteBurning();
                        BurstSparks();
                        burnable.DespawnObject();
                    }
                    else
                        BurningItemsList.Add(burnable);
                }
                else if (!isLit) {
                    burnable.DespawnObject();
                }
            }
            else {
                AddHealth(burnable.GetFuelAmount());
                burnable.DespawnObject();
            }
        }
    }

    void OnTriggerStay2D(Collider2D collider) {
        if (!isBurnt) {
            foreach (Item burnable in BurningItemsList) {
                burnable.StartBurn(Time.deltaTime * TimeManager.Instance.CurrentTimeSettings.timeMultiplier);

                if (burnable.IsBurntOut) {
                    BurningItemsList.Remove(burnable);
                    BurstSparks();
                    burnable.DespawnObject();
                    break;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider) {
        Item burnable = (Item)collider.gameObject.GetComponentInHierarchy<IBurnable>();

        if (burnable != null) {
            BurningItemsList.Remove(burnable);
            burnable.EndBurn();
        }
    }

    public void OnInteract() {
        if (!IsInteractable) return;

        CheckCampfireStatus();
    }

    void ConsumeFuel() {
        if (isBurnt || !isLit || !consumeFuel) return;
        
        if (!isOutOfFuel) {
            AddFuel(-(baseFuelConsumptionRate
                * (WeatherManager.Instance.IsRaining && WeatherManager.Instance.CurrentWeatherSettings.IsNotNull() ? WeatherManager.Instance.CurrentWeatherSettings.fuelConsuptionMultiplier : 1)
                * (TimeManager.Instance.CurrentTimeSettings.IsNotNull() ? TimeManager.Instance.InGameDeltaTime : Time.deltaTime)
            ));
        }
        else {
            BurnCampfire();
        }
    }

    public void CheckCampfireStatus() {
        if (isBurnt) return;

        if (!isLit) {
            LitFire();
        }
        else if (isLit) {
            UnlitFire();
        }
    }

    void LitFire() {
        isLit = true;
        FireEffect.Play();
        Anim.CrossFade(LitHash, 0f);
        fuelSlider.gameObject.SetActive(true);
        healthSlider.gameObject.SetActive(false);
        SmokeEffect.Stop();
        // BurnCollider.enabled = true;
        BurstSparks();
        AmbienceSource.Play();
    }

    void UnlitFire() {
        isLit = false;
        FireEffect.Stop();
        Anim.CrossFade(UnlitHash, 0f);
        fuelSlider.gameObject.SetActive(true);
        healthSlider.gameObject.SetActive(false);
        SmokeEffect.Play();
        // BurnCollider.enabled = false;
        AmbienceSource.Stop();
    }

    void BurnCampfire(bool burst = true) {
        isLit = false;
        isBurnt = true;
        FireEffect.Stop();
        Anim.CrossFade(BurntHash, 0f);
        fuelSlider.gameObject.SetActive(false);
        healthSlider.gameObject.SetActive(true);
        SmokeEffect.Play();
        // BurnCollider.enabled = false;
        SetHealth(0f);
        if (burst)
            BurstSparks();
        AmbienceSource.Stop();
    }

    void AddHealth(float healthAmount) {
        SetHealth(currentHealth + healthAmount);
        
        UpdateHealthParameters();

        if (currentHealth >= maxHealth) {
            RestoreCampfire();
        }
    }

    void SetHealth(float amount) {
        currentHealth = amount;
        currentHealth = currentHealth.Clamp(0, maxHealth);
    }

    void AddFuel(float fuelAmount) {
        SetFuel(currentFuelAmount + fuelAmount);
    }

    void SetFuel(float amount) {
        currentFuelAmount = amount;
        currentFuelAmount = currentFuelAmount.Clamp(0, maxFuelAmount);
    }

    void RestoreCampfire() {
        isBurnt = false;
        SetFuel(maxFuelAmount / 2f);
        UnlitFire();
    }

    void UpdateFuelParameters() {
        float fuelPercentage = CalculateFuelPercentage();
        fireEmission.rateOverTime = (fireAmountCurve.Evaluate(fuelPercentage) * baseFireAmount).ToIntRound();
        MainLight.pointLightOuterRadius = lightOuterRadius;
        MainLight.pointLightInnerRadius = (fireAmountCurve.Evaluate(fuelPercentage) * lightOuterRadius / 2).Clamp(0, lightOuterRadius);
        fuelSlider.value = fuelPercentage;
        fuelFillImage.color = isLit ? fuelLitColorGradient.Evaluate(fuelPercentage) : fuelUnlitColorGradient.Evaluate(fuelPercentage);
    }

    void UpdateHealthParameters() {
        if (currentFuelAmount > 0)
            isOutOfFuel = false;
        else
            isOutOfFuel = true;

        float healthPercentage = CalculateHealthPercentage();
        healthSlider.value = healthPercentage;
        healthFillImage.color = healthColorGradient.Evaluate(healthPercentage);
    }

    void BurstSparks() {
        SFXSource.PlayOneShot(AudioManager.Instance.GetSFXClip("FireCrack", true));
        SparkEffect.Play();
    }

    float CalculateHealthPercentage() {
        return currentHealth % (maxHealth + 0.001f) / (maxHealth + 0.001f);
    }

    float CalculateFuelPercentage() {
        return currentFuelAmount % (maxFuelAmount + 0.001f) / (maxFuelAmount + 0.001f);
    }
}
