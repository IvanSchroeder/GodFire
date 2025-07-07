using UnityEngine;

[CreateAssetMenu(fileName = "NewDraggableSettings", menuName = "Data/Interaction System/Draggable Settings")]
public class DraggableSettingsSO : ScriptableObject {
    public Material DropShadowMaterial;
    public FloatSO pickupOffsetY;
    public float velocity = 15f;
    public float pickupSpeed = 15f;
    public float fallDuration = 0.5f;
    public Vector2 pickupScale = new Vector2(1.2f, 1.2f);
    public float scaleSpeed = 0.3f;
    public int pickupVibrato = 1;
    public float pickupElasticity = 5f;
    public bool rotateItemOnPickup = true;
    public float rotationAmount = 15f;
    public float rotationSpeed = 15f;
    public int defaultSortingOrder = 0;
    public int pickupSortingOrder = 500;
}
