using System;
using DG.Tweening;
using UnityUtilities;
using UnityEngine;

[Serializable]
public class Item : MonoBehaviour, IDraggable, IBurnable {
    [SerializeField] SpriteRenderer itemSprite;
    [SerializeField] SpriteRenderer dropShadowSprite;
    [SerializeField] FloatSO ItemDespawnSeconds;
    [SerializeField] float spriteOffset = 0.25f;

    [Header("Drag Parameters")]
    [field: SerializeField] public Collider2D DragCollider { get; set; }
    [field: SerializeField] public DraggableSettingsSO DraggableSettings { get; set; }

    public bool IsDragging { get; set; }
    public bool IsTouchingGround { get; set; }
    public float DistanceToFloor { get; set; }
    public Vector2 MovementDelta { get; set; }
    public Vector2 RotationDelta { get; set; }

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

    Vector2 originalSpritePosition;
    Vector2 targetItemPosition;
    Vector2 itemVelocityRef;
    Vector2 spriteVelocity;
    int originalSortingOrder;

    CountdownTimer itemDespawnTimer;

    public static event Action OnItemBurnt = delegate {};

    public static readonly int MAINCOLOR = Shader.PropertyToID("_Color");
    public static readonly int ALPHA = Shader.PropertyToID("_Alpha");
    public static readonly int ALTITUDE = Shader.PropertyToID("_Altitude");
    public static readonly int SIZE = Shader.PropertyToID("_Size");

    void OnEnable() {
        itemDespawnTimer.Restart();
        itemDespawnTimer.OnTimerStop += DespawnObject;
    }

    void OnDisable() {
        itemDespawnTimer.Stop();
        itemDespawnTimer.OnTimerStop -= DespawnObject;
    }

    void Awake() {
        itemDespawnTimer = new CountdownTimer(ItemDespawnSeconds.Value, ref InteractionSystem.Instance.InteractableTimersManager);
    }

    void Start() {
        dropShadowSprite.material = new(DraggableSettings.DropShadowMaterial);

        BurnHealth = BurnAmount;
        IsBurntOut = false;
        IsBurning = false;
        targetItemPosition = transform.position;
        originalSortingOrder = DraggableSettings.defaultSortingOrder;
        originalSpritePosition = Vector2.zero.Add(y: spriteOffset);
        itemSprite.transform.localPosition = originalSpritePosition.Add(y: DraggableSettings.pickupOffsetY.Value);

        itemSprite.transform.DOLocalJump(originalSpritePosition, DraggableSettings.pickupOffsetY.Value, 1, DraggableSettings.fallDuration);
        itemSprite.transform.DOPunchScale(Vector2.one * 0.5f, DraggableSettings.scaleSpeed, DraggableSettings.pickupVibrato, DraggableSettings.pickupElasticity).SetEase(Ease.OutExpo);
    }

    void Update() {
        transform.position = Vector2.SmoothDamp(transform.position, targetItemPosition, ref itemVelocityRef, DraggableSettings.velocity * Time.deltaTime);
        if (IsDragging)
            itemSprite.transform.localPosition = Vector2.SmoothDamp(itemSprite.transform.localPosition, originalSpritePosition.Add(y: DraggableSettings.pickupOffsetY.Value), ref spriteVelocity, DraggableSettings.pickupSpeed * Time.deltaTime);

        DistanceToFloor = (transform.position.y - itemSprite.transform.position.y).AbsoluteValue();
        dropShadowSprite.material.SetFloat(ALTITUDE, DistanceToFloor);

        if (!DraggableSettings.rotateItemOnPickup) return;

        Vector2 movement = transform.position.ToVector2() - targetItemPosition;
        MovementDelta = Vector2.Lerp(MovementDelta, movement, 25 * Time.deltaTime);
        Vector2 movementRotation = (IsDragging ? MovementDelta : movement) * DraggableSettings.rotationAmount;
        RotationDelta = Vector3.Lerp(RotationDelta, movementRotation, DraggableSettings.rotationSpeed * Time.deltaTime);
        itemSprite.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Clamp(RotationDelta.x, -60, 60));
    }

    public void OnPickup(Vector3 pos) {
        IsDragging = true;
        DragCollider.enabled = false;

        itemSprite.transform.DOComplete();
        itemSprite.transform.DOScale(DraggableSettings.pickupScale, DraggableSettings.scaleSpeed).SetEase(Ease.OutExpo);

        itemSprite.sortingOrder = 500;

        AudioManager.Instance?.PlaySFX("GrassStep", true);

        itemDespawnTimer.Pause();
    }

    public void OnHold(Vector2 offsetPos) {
        targetItemPosition = offsetPos;
    }

    public void OnDrop(Vector3 pos) {
        IsDragging = false;
        DragCollider.enabled = true;

        targetItemPosition = pos;

        itemSprite.transform.DOComplete();
        itemSprite.transform.DOLocalMoveY(originalSpritePosition.y, DraggableSettings.fallDuration).SetEase(Ease.OutBounce);
        itemSprite.transform.DOScale(new Vector2(1, 1), DraggableSettings.scaleSpeed).SetEase(Ease.OutExpo);

        itemSprite.sortingOrder = originalSortingOrder;

        itemDespawnTimer.Restart();
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