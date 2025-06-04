using UnityEngine;
using UnityUtilities;

public interface IInteractable {
    Collider2D InteractionCollider { get; set; }
    bool IsInteractable { get; set; }
    bool HasInteractabilityCooldown { get; set; }
    CountdownTimer InteractabilityTimer { get; set; }
    float InteractabilityCooldownSeconds { get; set; }

    void OnInteract();
}