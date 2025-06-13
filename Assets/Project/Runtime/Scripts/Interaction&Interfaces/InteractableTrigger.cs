using UnityEngine;
using UnityEngine.EventSystems;
using UnityUtilities;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractableTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IHoverable {
    public IInteractable InteractableObject;
    public BoxCollider2D InteractionCollider;
    public bool IsInteractable;
    public bool OneTimeInteraction;
    public bool HasInteractabilityCooldown;
    public CountdownTimer InteractabilityTimer;
    public float InteractabilityCooldownSeconds;
    public PointerEventData.InputButton InteractionButton;
    public Vector2 TriggerOffset;
    public Vector2 TriggerSize;

    private void OnValidate() {
        InteractionCollider.offset = TriggerOffset;
        InteractionCollider.size = TriggerSize;
    }

    private void OnEnable() {
        if (HasInteractabilityCooldown) {
            InteractabilityTimer.OnTimerStart += () => DisableInteractability();
            InteractabilityTimer.OnTimerStop += () => EnableInteractability();
        }
    }

    private void OnDisable() {
        if (HasInteractabilityCooldown) {
            InteractabilityTimer.OnTimerStart -= () => DisableInteractability();
            InteractabilityTimer.OnTimerStop -= () => EnableInteractability();
            InteractionSystem.Instance.InteractableTimersManager?.DeregisterTimer(InteractabilityTimer);
        }
    }

    private void Awake() {
        InteractableObject = this.GetComponentInHierarchy<IInteractable>();

        if (HasInteractabilityCooldown)
            InteractabilityTimer = new CountdownTimer(InteractabilityCooldownSeconds, ref InteractionSystem.Instance.InteractableTimersManager);

        if (InteractionCollider.IsNull())
            InteractionCollider = GetComponent<BoxCollider2D>();

        InteractionCollider.isTrigger = true;

        InteractionCollider.offset = TriggerOffset;
        InteractionCollider.size = TriggerSize;
    }

    public void Init() {
        RestartInteractability();
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (!IsInteractable || eventData.button != InteractionButton) return;

        RestartInteractability();
        InteractableObject?.OnInteract();
    }

    public void OnPointerUp(PointerEventData eventData) { }

    public void OnPointerEnter(PointerEventData eventData) {
        InteractableObject?.OnHoverStart();
    }

    public void OnPointerExit(PointerEventData eventData) {
        InteractableObject?.OnHoverEnd();
    }

    public void EnableInteractability() => IsInteractable = true;
    public void DisableInteractability() => IsInteractable = false;
    void RestartInteractability() {
        if (HasInteractabilityCooldown) {
            InteractionSystem.Instance.InteractableTimersManager.RegisterTimer(InteractabilityTimer);
            InteractabilityTimer.Restart();
        }
    }
}
