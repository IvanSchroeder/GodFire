using UnityEngine;

public interface IDraggable {
    public Collider2D DragCollider { get; set; }
    public bool IsDragging { get; set; }
    public bool IsTouchingGround { get; set; }
    public Vector2 PickupOffset { get; set; }
    public float DistanceToFloor { get; set; }
    public float Velocity { get; set; }
    public float PickupSpeed { get; set; }
    public float FallDuration { get; set; }
    public Vector2 PickupScale { get; set; }
    public float ScaleSpeed { get; set; }
    public int PickupVibrato { get; set; }
    public float PickupElasticity { get; set; }
    public bool RotateItemOnPickup { get; set; }
    public Vector2 MovementDelta { get; set; }
    public Vector2 RotationDelta { get; set; }
    public float RotationAmount { get; set; }
    public float RotationSpeed { get; set; }

    void OnPickup(Vector3 pos);
    void OnHold(Vector2 offsetPos);
    void OnDrop(Vector3 pos);
}
