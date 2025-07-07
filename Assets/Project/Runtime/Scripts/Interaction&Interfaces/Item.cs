using System;
using DG.Tweening;
using UnityUtilities;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class ItemData {
    [SerializeField] string _name = "NewItem";
    [SerializeField] UniqueID _id = new();
    [SerializeField] int _itemID = -1;
    [SerializeField] Vector3 _worldPosition = Vector3.zero;

    public string Name {
        get => _name;
        set => _name = value;
    }

    public UniqueID ID {
        get => _id;
        set => _id = value;
    }

    public int ItemID {
        get => _itemID;
        set => _itemID = value;
    }

    public Vector3 WorldPosition {
        get => _worldPosition;
        set => _worldPosition = value;
    }

    public ItemData() {}
}

[Serializable]
public class Item : MonoBehaviour, IDraggable, IBurnable, IHoverable {
    [SerializeField] public ItemData itemData = new();
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

    [Header("Burnable Settings")]
    [field: SerializeField] public BurnableTrigger BurnableTrigger { get; set; }
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

    public CountdownTimer itemDespawnTimer;

    public float timeTillDespawn = 0;

    public static event Action OnItemBurnt = delegate {};
    public event Action OnItemPickup = delegate {};
    public event Action OnItemDrop = delegate {};

    public static readonly int MAIN_COLOR = Shader.PropertyToID("_Color");
    public static readonly int ALPHA = Shader.PropertyToID("_Alpha");
    public static readonly int ALTITUDE = Shader.PropertyToID("_Altitude");
    public static readonly int SIZE = Shader.PropertyToID("_Size");

    void OnEnable() {
        itemDespawnTimer.Restart();
        itemDespawnTimer.OnTimerStop += DespawnObject;
    }

    void OnDisable() {
        itemDespawnTimer.OnTimerStop -= DespawnObject;
        itemDespawnTimer.Stop();

        WorldGenerator.Instance.DeregisterItem(this);
        WorldGenerator.Instance.DeregisterItemData(itemData);
    }

    void Awake() {
        itemDespawnTimer = new CountdownTimer(ItemDespawnSeconds.Value, ref InteractionSystem.Instance.InteractableTimersManager);

        if (BurnableTrigger.IsNull()) BurnableTrigger = GetComponent<BurnableTrigger>();
        BurnableTrigger?.Init();
    }

    void Start() {
        dropShadowSprite.material = new(DraggableSettings.DropShadowMaterial);

        BurnHealth = BurnAmount;
        IsBurntOut = false;
        IsBurning = false;
        targetItemPosition = transform.position;
        originalSpritePosition = Vector2.zero.Add(y: spriteOffset);
        itemSprite.transform.localPosition = originalSpritePosition.Add(y: DraggableSettings.pickupOffsetY.Value);

        itemSprite.transform.DOLocalJump(originalSpritePosition, DraggableSettings.pickupOffsetY.Value, 1, DraggableSettings.fallDuration);
        itemSprite.transform.DOPunchScale(Vector2.one * 0.5f, DraggableSettings.scaleSpeed, DraggableSettings.pickupVibrato, DraggableSettings.pickupElasticity).SetEase(Ease.OutExpo);
    }

    void Update() {
        transform.position = Vector2.SmoothDamp(transform.position, targetItemPosition, ref itemVelocityRef, DraggableSettings.velocity * Time.deltaTime);
        UpdateDropShadow();
        RotateItem();
        timeTillDespawn = itemDespawnTimer.IsNotNull() ? itemDespawnTimer.GetTime() : 0;

        itemData.WorldPosition = transform.position;
    }

    void UpdateDropShadow() {
        DistanceToFloor = (transform.position.y - itemSprite.transform.position.y).AbsoluteValue();
        dropShadowSprite.material.SetFloat(ALTITUDE, DistanceToFloor);
    }

    void RotateItem() {
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
        itemSprite.transform.DOLocalMove(originalSpritePosition.Add(y: DraggableSettings.pickupOffsetY.Value), DraggableSettings.pickupSpeed).SetEase(Ease.OutExpo); 
        itemSprite.transform.DOScale(DraggableSettings.pickupScale, DraggableSettings.scaleSpeed).SetEase(Ease.OutExpo);

        itemSprite.sortingOrder = DraggableSettings.pickupSortingOrder;

        AudioManager.Instance?.PlaySFX("GrassStep", true);

        itemDespawnTimer.Pause();

        OnItemPickup?.Invoke();
    }

    public void OnHold(Vector2 offsetPos) {
        targetItemPosition = offsetPos;
    }

    public async void OnDrop(Vector3 pos) {
        IsDragging = false;
        DragCollider.enabled = true;

        targetItemPosition = pos;

        itemSprite.sortingOrder = DraggableSettings.defaultSortingOrder;

        itemDespawnTimer.Restart(ItemDespawnSeconds.Value);

        itemSprite.transform.DOComplete();
        itemSprite.transform.DOScale(Vector2.one, DraggableSettings.scaleSpeed).SetEase(Ease.OutExpo);
        await itemSprite.transform.DOLocalMoveY(originalSpritePosition.y, DraggableSettings.fallDuration).SetEase(Ease.OutBounce).IsComplete(); 

        OnItemDrop?.Invoke();
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

    public void OnPointerEnter(PointerEventData eventData) {
        itemSprite.sortingOrder = DraggableSettings.pickupSortingOrder;
    }

    public void OnPointerExit(PointerEventData eventData) {
        itemSprite.sortingOrder = DraggableSettings.defaultSortingOrder;
    }
}