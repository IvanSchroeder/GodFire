using UnityUtilities;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class InteractionSystem : Singleton<InteractionSystem> {
    [Header("References")]
    [Space(5f)]
    Item draggedObject = null;
    Camera mainCamera;
    public LayerMask DraggableLayer;
    public LayerMask CampfireLayer;
    public LayerMask InteractableLayer;
    public Image GodHandImage;
    public Image CursorImage;
    public Sprite IdleGodHandSprite;
    public Sprite DragGodHandSprite;
    public TextMeshProUGUI SelectedTileHeightValueText;
    public TextMeshProUGUI HoveredTileHeightValueText;

    [Space(10f)]
    [Header("Grid Parameters")]
    [Space(5f)]

    public SpriteRenderer mouseGrid;
    public SpriteRenderer selectedCellGrid;
    [SerializeField] FloatSO pickupOffsetY;
    [SerializeField] float dragTime = 5f;
    [SerializeField] float normalTime = 1f;
    [SerializeField, Range(1, 32)] int mouseGridSize = 4;
    [SerializeField] bool renderMouseGrid = true;
    [SerializeField] bool renderSelectionGrid = true;
    [SerializeField] bool snapGrid = true;
    [SerializeField] public float gridSmoothness = 10f;
    [SerializeField] public Vector3 mouseGridOffset;

    Vector2 _offset;
    Vector2 _mouseScreenPosition;
    Vector3 _mouseWorldPosition;
    Vector3 _mouseGridPosition;
    Vector3Int _mouseCellPosition;
    Vector3 _selectedCellPosition;
    Vector3 _previousSelectedCellPosition;
    Vector2 _currentHandPosition;
    Vector2 _targetHandPosition;
    Vector2 _handVelocity;
    Vector3 _gridVelocity;

    public TimerManager InteractableTimersManager = new();

    void OnDisable() {
        InteractableTimersManager.DisposeTimers();
    }

    protected override void Awake() {
        base.Awake();

        InteractableTimersManager = new();
    }

    void Start() {
        mainCamera = this.GetMainCamera();
        CursorImage.transform.position = Vector2.zero;
    }

    void Update() {
        InteractableTimersManager.UpdateTimers(Time.deltaTime);

        _mouseScreenPosition = Input.mousePosition;
        _mouseWorldPosition = mainCamera.ScreenToWorld(_mouseScreenPosition).With(z: 0);
        _mouseCellPosition = WorldGenerator.Instance.GroundTilemap.WorldToCell(_mouseWorldPosition);
        _mouseGridPosition = WorldGenerator.Instance.GroundTilemap.CellToWorld(_mouseCellPosition) + mouseGridOffset;

        if (Input.GetMouseButtonDown(0)) {
            // var matrix = Matrix4x4.TRS(Vector3.zero + (Vector3.up * 2f), Quaternion.identity, Vector3.one * 5f);
            // WorldGenerator.instance.GroundTilemap.SetTransformMatrix(currentCell, matrix);
            // OffsetTileHeight(WorldGenerator.instance.GroundTilemap, _currentCell, 0.1f);
            _selectedCellPosition = _mouseGridPosition;
            HandleSelectionGrid();
        }

        if (Input.GetMouseButtonDown(0)) {
            SwapGodHand(DragGodHandSprite);
            RaycastHit2D interactableHit = Physics2D.Raycast(_mouseWorldPosition, Vector2.zero, default, InteractableLayer);

            if (interactableHit && interactableHit.transform.TryGetComponent<IInteractable>(out var interactable)) {
                interactable.OnInteract();
            }
        }
        else if (Input.GetMouseButtonUp(0)) {
            SwapGodHand(IdleGodHandSprite);
        }

        if (Input.GetMouseButtonDown(1)) {
            SwapGodHand(DragGodHandSprite);
            RaycastHit2D draggableHit = Physics2D.Raycast(_mouseWorldPosition, Vector2.zero, default, DraggableLayer);

            if (draggableHit) {
                draggedObject = (Item)draggableHit.transform.GetComponentInHierarchy<IDraggable>();
                _offset = draggedObject.transform.position - _mouseWorldPosition;
                draggedObject.OnPickup(_mouseWorldPosition + _offset.ToVector3());
            }
        }
        else if (Input.GetMouseButtonUp(1)) {
            SwapGodHand(IdleGodHandSprite);
            draggedObject?.OnDrop(_mouseWorldPosition + _offset.ToVector3());
            draggedObject = null;
        }

        if (draggedObject != null) {
            _targetHandPosition = _mouseWorldPosition + (Vector3.up * pickupOffsetY.Value);
            _currentHandPosition = Vector2.SmoothDamp(_currentHandPosition, _targetHandPosition, ref _handVelocity, dragTime * Time.deltaTime);
            draggedObject?.OnHold(_mouseWorldPosition + _offset.ToVector3());
        }
        else {
            _targetHandPosition = _mouseWorldPosition;
            _currentHandPosition = Vector2.SmoothDamp(_currentHandPosition, _targetHandPosition, ref _handVelocity, normalTime * Time.deltaTime);
        }

        CursorImage.transform.position = _mouseWorldPosition;
        GodHandImage.transform.position = _currentHandPosition;

        HandleMouseGrid();
    }

    void SwapGodHand(Sprite _sprite) {
        GodHandImage.sprite = _sprite;
    }

    void HandleMouseGrid() {
        // Vector3 targetPosition = snapGrid ? _mouseGridPosition : _mouseWorldPosition;
        // mouseGrid.transform.localScale = new Vector3(mouseGridSize, mouseGridSize / 2, 1);
        // mouseGrid.transform.position = Vector3.SmoothDamp(mouseGrid.transform.position, targetPosition, ref _gridVelocity, gridSmoothness * Time.deltaTime);
    }

    void HandleSelectionGrid() {
        // if (_selectedCellPosition != _previousSelectedCellPosition) {
        //     selectedCellGrid.enabled = true;
        //     _previousSelectedCellPosition = _selectedCellPosition;
        // }
        // else {
        //     ToggleSelectionGrid();
        // }

        // selectedCellGrid.transform.position = _selectedCellPosition;

        // Vector3Int chunkPos = WorldDataHelper.ChunkPositionFromTileCoords(WorldGenerator.Instance.chunkSize, _mouseCellPosition);
        // Chunk chunk = WorldGenerator.Instance.WorldData.ChunkData
        //     .GetChunkAt(WorldDataHelper.ChunkIDFromChunkPosition(WorldGenerator.Instance.chunkSize, chunkPos));

        // if (chunk.IsNotNull())
        //     SelectedTileHeightValueText.text = $"Selected Tile Height: {chunk.GetLocalNoiseValueAt(_mouseCellPosition - chunk.WorldPosition):f3}";
        // else if (!selectedCellGrid.enabled)
        //     SelectedTileHeightValueText.text = $"Selected Tile Height: -,-";
    }

    void ToggleMouseGrid() {
        renderMouseGrid = renderMouseGrid.Toggle();
        mouseGrid.enabled = mouseGrid.enabled.Toggle();
    }

    void ToggleSelectionGrid() {
        renderSelectionGrid = renderSelectionGrid.Toggle();
        selectedCellGrid.enabled = renderSelectionGrid;
    }

    private void OffsetTileHeight(Tilemap map, Vector3Int coordinate, float heightOffset) {
        TileBase tile = map.GetTile<TileBase>(coordinate);
        float height = map.GetTransformMatrix(coordinate).GetPosition().y;

        Matrix4x4 tileTransform = Matrix4x4.Translate(new Vector3(0, height + heightOffset, 0));
        TileChangeData tileChangeData = new TileChangeData() {
            position = coordinate,
            tile = tile,
            transform = tileTransform
        };

        map.SetTile(tileChangeData, false);
    }
}