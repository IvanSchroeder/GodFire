using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

[RequireComponent(typeof(CircleCollider2D))]
public class BurnerTrigger : MonoBehaviour {
    public IBurner BurnerObject;
    public CircleCollider2D BurnerCollider;
    public bool CanBurn;
    public Vector2 TriggerOffset;
    public float TriggerRadius;

    private void OnValidate() {
        BurnerCollider.offset = TriggerOffset;
        BurnerCollider.radius = TriggerRadius;
    }

    public void Init() {
        BurnerObject = this.GetComponentInHierarchy<IBurner>();

        if (BurnerCollider.IsNull())
            BurnerCollider = GetComponent<CircleCollider2D>();

        BurnerCollider.isTrigger = true;
        SetCombustibility(true);

        BurnerCollider.offset = TriggerOffset;
        BurnerCollider.radius = TriggerRadius;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        // Item burnable = (Item)collider.gameObject.GetComponentInHierarchy<IBurnable>();
        Item burnable = (Item)collider.gameObject.GetComponentInHierarchy<BurnableTrigger>().BurnableObject;

        if (burnable) {
            Debug.Log($"Entered Burn Trigger: {burnable.name} in {BurnerObject}");
            BurnerObject.OnItemEntered<Item>(burnable);
        }
        else {
            Debug.Log($"No Burnable detected!");
        }
    }

    // void OnTriggerStay2D(Collider2D collider) {
    //     if (!LocalCampfireData.IsBurnt) {
    //         foreach (Item burnable in BurningItemsList) {
    //             burnable.StartBurn(Time.deltaTime * TimeManager.Instance.CurrentTimeSettings.timeMultiplier);

    //             if (burnable.IsBurntOut) {
    //                 BurningItemsList.Remove(burnable);
    //                 BurstSparks();
    //                 burnable.DespawnObject();
    //                 break;
    //             }
    //         }
    //     }
    // }

    void OnTriggerExit2D(Collider2D collider) {
        // Item burnable = (Item)collider.gameObject.GetComponentInHierarchy<IBurnable>();

        if (collider.gameObject.GetComponentInHierarchy<IBurnable>() is Item burnable)

        if (burnable != null) {
            Debug.Log($"Exited Burn Trigger: {burnable.name} from {BurnerObject}");
            BurnerObject.OnItemExited<Item>(burnable);
        }
    }

    public void SetCombustibility(bool state) {
        CanBurn = state;
        BurnerCollider.enabled = CanBurn;
    }
}
