using UnityEngine;
using UnityUtilities;

[RequireComponent(typeof(CircleCollider2D))]
public class BurnableTrigger : MonoBehaviour {
    public IBurnable BurnableObject;
    public CircleCollider2D BurnableCollider;

    public Vector2 TriggerOffset;
    public float TriggerRadius;

    private void OnDisable() {
        var item = BurnableObject as Item;
        item.OnItemPickup -= () => SetBurnableState(false);
        item.OnItemDrop -= () => SetBurnableState(true);
    }

    private void OnValidate() {
        BurnableCollider.offset = TriggerOffset;
        BurnableCollider.radius = TriggerRadius;
    }

    public void Init() {
        BurnableObject = this.GetComponentInHierarchy<IBurnable>();
        var item = BurnableObject as Item;
        item.OnItemPickup += () => SetBurnableState(false);
        item.OnItemDrop += () => SetBurnableState(true);

        if (BurnableCollider.IsNull()) BurnableCollider = GetComponent<CircleCollider2D>();

        BurnableCollider.isTrigger = true;
        BurnableCollider.enabled = true;

        BurnableCollider.offset = TriggerOffset;
        BurnableCollider.radius = TriggerRadius;
    }

    public void SetBurnableState(bool state) {
        BurnableCollider.enabled = state;
    }
}
