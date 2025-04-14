using ExtensionMethods;
using UnityEngine;
using UnityEngine.UI;

public class DragObject : MonoBehaviour {
    Item draggedObject = null;
    Camera mainCamera;
    public LayerMask DraggableLayer;
    public LayerMask CampfireLayer;
    public Image CursorImage;
    public Sprite IdleCursorSprite;
    public Sprite DragCursorSprite;

    Vector2 offset;
    Vector2 mousePosition;
    Vector2 mouseWorldPosition;
    Vector2 targetCursorPosition;
    Vector2 cursorVelocity;

    void Start() {
        mainCamera = this.GetMainCamera();
        CursorImage.transform.position = Vector2.zero;
        Cursor.visible = false;
    }

    void Update() {
        mousePosition = Input.mousePosition;
        mouseWorldPosition = mainCamera.ScreenToWorldV2(mousePosition);

        if (Input.GetMouseButtonDown(0)) {
            SwapCursor(DragCursorSprite);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, default, DraggableLayer);

            if (hit) {
                draggedObject = (Item)hit.transform.GetComponentInHierarchy<IDraggable>();
                offset = draggedObject.transform.position.ToVector2() - mouseWorldPosition;
                draggedObject.OnPickup(mouseWorldPosition + offset);
            }
            else {
                RaycastHit2D campfireHit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, default, CampfireLayer);

                if (campfireHit) {
                    Campfire campf = campfireHit.transform.GetComponentInHierarchy<Campfire>();
                    campf.CheckCampfireStatus();
                }
            }

        }
        else if (Input.GetMouseButtonUp(0)) {
            SwapCursor(IdleCursorSprite);
            draggedObject?.OnDrop(mouseWorldPosition + offset);
            draggedObject = null;
        }

        if (draggedObject != null) {
            targetCursorPosition = Vector2.SmoothDamp(targetCursorPosition, mouseWorldPosition + (Vector2.up * 0.5f), ref cursorVelocity, Time.deltaTime * 5f);
            draggedObject?.OnHold(mouseWorldPosition + offset);
        }
        else {
            targetCursorPosition = Vector2.SmoothDamp(targetCursorPosition, mouseWorldPosition, ref cursorVelocity, Time.deltaTime * 1f);
        }

        CursorImage.transform.position = targetCursorPosition;
    }

    void SwapCursor(Sprite _sprite) {
        CursorImage.sprite = _sprite;
    }
}