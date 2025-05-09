using System;
using DG.Tweening;
using UnityUtilities;
using UnityEngine;

public class Item : MonoBehaviour, IDraggable, IBurnable {
    [SerializeField] SpriteRenderer itemSprite;
    [SerializeField] SpriteRenderer dropShadowSprite;
    [SerializeField] FloatSO ItemDespawnSeconds;

    [Header("Drag Parameters")]
    [field: SerializeField] public Collider2D DragCollider { get; set; }
    public bool IsDragging { get; set; }
    public bool IsTouchingGround { get; set; }
    [field: SerializeField] public Vector2 PickupOffset { get; set; }
    public float DistanceToFloor { get; set; }
    [field: SerializeField] public float Velocity { get; set; }
    [field: SerializeField] public float PickupSpeed { get; set; }
    [field: SerializeField] public float FallDuration { get; set; }
    [field: SerializeField] public Vector2 PickupScale { get; set; }
    [field: SerializeField] public float ScaleSpeed { get; set; }
    [field: SerializeField] public int PickupVibrato { get; set; }
    [field: SerializeField] public float PickupElasticity { get; set; }
    [field: SerializeField] public bool RotateItemOnPickup { get; set; }
    public Vector2 MovementDelta { get; set; }
    public Vector2 RotationDelta { get; set; }
    [field: SerializeField] public float RotationAmount { get; set; }
    [field: SerializeField] public float RotationSpeed { get; set; }

    [Header("Burn Parameters")]
    [field: SerializeField] public bool IsBurning { get; set; }
    [field: SerializeField] public bool IsBurntOut { get; set; }
    [field: SerializeField] public bool BurnsInstantly { get; set; }
    [field: SerializeField] public ParticleSystem BurnEffect { get; set; }
    [field: SerializeField] public ParticleSystem SparkEffect { get; set; }
    [field: SerializeField] public float BurnHealth { get; set; }
    [field: SerializeField] public float BurnAmount { get; set; }
    [field: SerializeField] public float BurnRate { get; set; }
    [field: SerializeField] public float AdditionalFuelAmount { get; set; }

    Vector2 targetItemPosition;
    Vector2 itemVelocityRef;
    Vector2 spriteVelocity;
    int originalSortingOrder;

    CountdownTimer itemDespawnTimer;

    public static event Action OnItemBurnt = delegate {};

    public static readonly int MainColor_Property = Shader.PropertyToID("_Color");
    public static readonly int Alpha_Property = Shader.PropertyToID("_Alpha");
    public static readonly int Altitude_Property = Shader.PropertyToID("_Altitude");

    void OnEnable() {
        itemDespawnTimer.OnTimerStop += DespawnObject;
    }

    void OnDisable() {
        itemDespawnTimer.OnTimerStop -= DespawnObject;
    }

    void Awake() {
        itemDespawnTimer = new CountdownTimer(ItemDespawnSeconds.Value);
    }

    void Start() {
        BurnHealth = BurnAmount;
        IsBurntOut = false;
        IsBurning = false;
        targetItemPosition = transform.position;
        originalSortingOrder = itemSprite.sortingOrder;

        itemSprite.transform.localPosition = PickupOffset;
        // itemSprite.transform.DOLocalMoveY(0, FallDuration).SetEase(Ease.OutBounce);
        itemSprite.transform.DOLocalJump(Vector2.zero, PickupOffset.y, 1, FallDuration);
        itemSprite.transform.DOPunchScale(Vector2.one * 0.5f, ScaleSpeed, PickupVibrato, PickupElasticity).SetEase(Ease.OutExpo);

        itemDespawnTimer.Restart();
    }

    void Update() {
        transform.position = Vector2.SmoothDamp(transform.position, targetItemPosition, ref itemVelocityRef, Velocity * Time.deltaTime);
        if (IsDragging)
            itemSprite.transform.localPosition = Vector2.SmoothDamp(itemSprite.transform.localPosition, PickupOffset, ref spriteVelocity, PickupSpeed * Time.deltaTime);

        DistanceToFloor = (transform.position.y - itemSprite.transform.position.y).AbsoluteValue();
        dropShadowSprite.material.SetFloat(Altitude_Property, DistanceToFloor);

        if (!RotateItemOnPickup) return;

        Vector2 movement = transform.position.ToVector2() - targetItemPosition;
        MovementDelta = Vector2.Lerp(MovementDelta, movement, 25 * Time.deltaTime);
        Vector2 movementRotation = (IsDragging ? MovementDelta : movement) * RotationAmount;
        RotationDelta = Vector3.Lerp(RotationDelta, movementRotation, RotationSpeed * Time.deltaTime);
        itemSprite.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Clamp(RotationDelta.x, -60, 60));

        itemDespawnTimer.Tick(Time.deltaTime);
    }

    public void OnPickup(Vector3 pos) {
        IsDragging = true;
        DragCollider.enabled = false;

        itemSprite.transform.DOComplete();
        itemSprite.transform.DOScale(PickupScale, ScaleSpeed).SetEase(Ease.OutExpo);

        itemSprite.sortingOrder = 500;

        AudioManager.Instance?.PlaySFX("GrassStep", true);

        itemDespawnTimer.Reset();
    }

    public void OnHold(Vector2 offsetPos) {
        targetItemPosition = offsetPos;
    }

    public void OnDrop(Vector3 pos) {
        IsDragging = false;
        DragCollider.enabled = true;

        targetItemPosition = pos;

        itemSprite.transform.DOComplete();
        itemSprite.transform.DOLocalMoveY(0, FallDuration).SetEase(Ease.OutBounce);
        itemSprite.transform.DOScale(new Vector2(1, 1), ScaleSpeed).SetEase(Ease.OutExpo);

        itemSprite.sortingOrder = originalSortingOrder;

        itemDespawnTimer.Start();
    }

    public float GetFuelAmount() => AdditionalFuelAmount;

    public void StartBurn(float deltaTime) {
        if (IsBurntOut) return;

        if (BurnHealth <= 0) {
            CompleteBurning();
        }
        else {
            IsBurning = true;
            BurnHealth -= BurnRate * deltaTime;
        }
        
        BurnHealth = BurnHealth.Clamp(0, BurnAmount);

        itemDespawnTimer.Pause();
    }

    public void EndBurn() {
        if (IsBurntOut) return;

        IsBurning = false;
    }

    public void CompleteBurning() {
        IsBurning = false;
        IsBurntOut = true;

        transform.DOComplete();
        itemSprite.transform.DOComplete();

        OnItemBurnt?.Invoke();
    }

    public void DespawnObject() {
        transform.DOComplete();
        itemSprite.transform.DOComplete();
        gameObject.Destroy();
    }
}