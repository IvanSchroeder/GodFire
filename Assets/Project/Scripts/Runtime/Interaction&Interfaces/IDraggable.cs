using UnityEngine;

public interface IDraggable {
    public Collider2D DragCollider { get; set; }
    public DraggableSettingsSO DraggableSettings { get; set; }

    public bool IsDragging { get; set; }
    public bool IsTouchingGround { get; set; }
    public float DistanceToFloor { get; set; }
    public Vector2 MovementDelta { get; set; }
    public Vector2 RotationDelta { get; set; }

    void OnPickup(Vector3 pos);
    void OnHold(Vector2 offsetPos);
    void OnDrop(Vector3 pos);
}
