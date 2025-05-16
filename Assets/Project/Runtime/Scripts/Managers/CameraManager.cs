using UnityEngine;
using UnityUtilities;
using System;
using GD.MinMaxSlider;
using TMPro;
using UnityEngine.Events;
using DG.Tweening;

public class CameraManager : Singleton<CameraManager> {
    Camera mainCamera;

    [Header("Grid Settings")]
    [Space(5f)]
    [SerializeField] SpriteRenderer cameraGrid;
    [SerializeField] bool renderGrid = true;
    [SerializeField] bool snapGrid = true;
    [SerializeField] float gridSmoothness = 10f;
    [SerializeField] Vector3 cameraGridOffset;
    [SerializeField] Vector3 chunkGridOffset;
    [SerializeField] bool renderChunkGrid = true;
    [SerializeField] SpriteRenderer chunkGrid;

    [Header("Panning Settings")]
    [Space(5f)]
    [Range(0f, 100f)] public float panSpeed = 20f;
    public float panSmoothness = 10f;
    public float cameraAltitude = 10f;
    [Range(1f, 3f)] public float horizontalPanningFactor = 1f;
    [Range(1f, 3f)] public float verticalPanningFactor = 2f;

    [Space(10f)]

    [Header("Dragging Settings")]
    [Space(5f)]
    public bool canDrag = false;
    [Range(0f, 100f)] public float dragSpeed = 20f;
    public float dragSmoothness = 10f;
    [Range(1f, 3f)] public float horizontalDraggingFactor = 1;
    [Range(1f, 3f)] public float verticalDraggingFactor = 2;
    [Range(0f, 1f)] public float draggingFactor = 0.01f;
    [SerializeField] TextMeshProUGUI currentTilePositionIntText;
    [SerializeField] TextMeshProUGUI currentChunkPositionIntText;
    [SerializeField] TextMeshProUGUI currentWorldPositionText;

    Vector3 _defaultPosition = new Vector3(0, 0, -10);
    Vector2 _cameraMoveDirection;
    Vector3 _cameraWorldPosition;
    Vector3 _cameraTargetPosition;
    Vector3 _cameraVelocity;
    bool _isDraggingCamera;
    Vector3 _mousePosition;
    Vector3 _mouseDelta;
    Vector3 _gridVelocity;

    Vector3Int _cameraTilePosition;
    Vector3Int _cameraChunkPosition;
    Vector3 _cameraChunkCenterPosition;
    Vector3Int _cameraLastChunkPosition;

    public Vector3Int CameraTilePosition {
        get => _cameraTilePosition;
        set => _cameraTilePosition = value;
    }
    
    public Vector3Int CameraChunkPosition {
        get => _cameraChunkPosition;
        set => _cameraChunkPosition = value;
    }

    [Space(10f)]
    [Header("Zoom Settings")]
    [Space(5f)]
    public bool canZoom = true;
    public float defaultZoom = 6f;
    public float zoomSpeed = 6;
    public float zoomSmoothness = 5;
    float minZoom => defaultZoom * zoomRange.x;
    float maxZoom => defaultZoom * zoomRange.y;
    [MinMaxSlider(0.1f, 3f)] public Vector2 zoomRange = new Vector2(0.5f, 2f);
    [Range(0.1f, 2f)] public float zoomStep = 1f;
    [SerializeField] TextMeshProUGUI currentZoomText;

    float _currentZoom;
    float _targetZoom;

    // public UnityEvent OnChunkCrossed;
    public static Action OnChunkCrossed;

    void Start() {
        mainCamera = this.GetMainCamera();
        _defaultPosition = WorldGenerator.instance.GroundTilemap.CellToWorld(new Vector3Int(
            WorldGenerator.instance.chunkSize / 2,
            WorldGenerator.instance.chunkSize / 2,
            0));
        _defaultPosition.z = -cameraAltitude;
        ResetCameraPosition();

        defaultZoom = Mathf.Clamp(defaultZoom, minZoom, maxZoom);
        _currentZoom = defaultZoom;
        ResetCameraZoom();

        _cameraChunkPosition = WorldDataHelper.ChunkPositionFromTileCoords(WorldGenerator.instance.chunkSize, new Vector3Int(0, 0, 0));
    }

    void Update() {
        CalculatePanning();
        PanCamera();

        CalculateZoom();
        ZoomCamera();

        HandleCameraGrid();
        HandleChunkGrid();
    }


    void CalculatePanning() {
        _isDraggingCamera = Input.GetMouseButton(1) && canDrag;

        if (Input.GetMouseButtonDown(3)) {
            ResetCameraPosition();
        }
        
        if (_isDraggingCamera) {
            _mouseDelta = Input.mousePositionDelta;
            _cameraMoveDirection = new Vector3(_mouseDelta.x / horizontalDraggingFactor, _mouseDelta.y / verticalDraggingFactor);
            _cameraTargetPosition += (_cameraMoveDirection * -dragSpeed * draggingFactor / CalculatePercentOfZoom(_currentZoom)).ToVector3();
        }
        else {
            _cameraMoveDirection = new Vector3(Input.GetAxis("Horizontal") / horizontalPanningFactor, Input.GetAxis("Vertical") / verticalPanningFactor);
            _cameraTargetPosition += (_cameraMoveDirection * panSpeed * Mathf.InverseLerp(1f * _currentZoom, 0.001f * _currentZoom, CalculatePercentOfZoom(_currentZoom)) * Time.deltaTime).ToVector3().With(z: -cameraAltitude);
        }

        // _cameraTargetPosition = _cameraTargetPosition.Clamp(new Vector3(-20, -20, -cameraAltitude), new Vector3(20, 20, -cameraAltitude));
        _cameraWorldPosition = Vector3.SmoothDamp(_cameraWorldPosition, _cameraTargetPosition, ref _cameraVelocity, (_isDraggingCamera ? dragSmoothness : panSmoothness) * Time.deltaTime).With(z: -cameraAltitude);;
        _cameraWorldPosition = _cameraWorldPosition.With(
            x: float.IsNaN(_cameraWorldPosition.x) || float.IsInfinity(_cameraWorldPosition.x) ? 0 : _cameraWorldPosition.x,
            y: float.IsNaN(_cameraWorldPosition.y) || float.IsInfinity(_cameraWorldPosition.y) ? 0 : _cameraWorldPosition.y,
            z: -cameraAltitude
        );
        
        _cameraTilePosition = WorldGenerator.instance.GroundTilemap.WorldToCell(_cameraTargetPosition).With(z: 0);
        _cameraChunkPosition = WorldDataHelper.ChunkPositionFromTileCoords(WorldGenerator.instance.chunkSize, _cameraTilePosition);
        _cameraChunkCenterPosition = WorldGenerator.instance.GroundTilemap.CellToWorld(new Vector3Int(
            _cameraChunkPosition.x + (WorldGenerator.instance.chunkSize / 2),
            _cameraChunkPosition.y + (WorldGenerator.instance.chunkSize / 2),
            0));

        if (_cameraChunkPosition != _cameraLastChunkPosition) {
            _cameraLastChunkPosition = _cameraChunkPosition;
            OnChunkCrossed?.Invoke();
        }
    }

    void PanCamera() {
        mainCamera.transform.position = _cameraWorldPosition.With(z: -cameraAltitude);

        currentTilePositionIntText.text = $"Tile: {_cameraTilePosition.x},{_cameraTilePosition.y},{0}";
        currentChunkPositionIntText.text = $"Chunk: {_cameraChunkPosition.x / WorldGenerator.instance.chunkSize},{_cameraChunkPosition.y / WorldGenerator.instance.chunkSize},{0}";
        currentWorldPositionText.text = $"World: {_cameraTargetPosition.x:f2},{_cameraTargetPosition.y:f2},{0}";
    }

    void CalculateZoom() {
        if (Input.mouseScrollDelta.y < 0) {
            _targetZoom += zoomStep;
        }
        else if (Input.mouseScrollDelta.y > 0) {
            _targetZoom -= zoomStep;
        }

        if (Input.GetKeyDown(KeyCode.Mouse2)) {
            ResetCameraZoom();
        }

        _targetZoom = Mathf.Clamp(_targetZoom, minZoom, maxZoom);
    }

    void ZoomCamera() {
        _currentZoom = Mathf.Lerp(_currentZoom, canZoom ? _targetZoom : defaultZoom, zoomSmoothness * Time.deltaTime);
        mainCamera.orthographicSize = _currentZoom;
        currentZoomText.text = $"Zoom: {CalculatePercentOfZoom(_currentZoom):f2}";
    }

    public void ResetCameraPosition() => _cameraTargetPosition = _defaultPosition;
    void ResetCameraZoom() => _targetZoom = defaultZoom;

    float CalculatePercentOfZoom(float current) {
        return (current % maxZoom / maxZoom) * 2;
    }

    void HandleCameraGrid() {
        if (renderGrid) {
            cameraGrid.enabled = true;
            Vector3 targetPosition = snapGrid ? (WorldGenerator.instance.GroundTilemap.CellToWorld(_cameraTilePosition) + cameraGridOffset).With(z: 0) : _cameraWorldPosition.With(z: 0);
            cameraGrid.transform.position = Vector3.SmoothDamp(cameraGrid.transform.position, targetPosition, ref _gridVelocity, gridSmoothness * Time.deltaTime);
        }
        else {
            cameraGrid.enabled = false;
        }
    }

    void HandleChunkGrid() {
        if (renderChunkGrid) {
            chunkGrid.enabled = true;
            Vector3 targetPosition = (_cameraChunkCenterPosition + chunkGridOffset).With(z: 0);
            chunkGrid.transform.position = targetPosition;
            chunkGrid.transform.localScale = new Vector3Int(WorldGenerator.instance.chunkSize, WorldGenerator.instance.chunkSize / 2);
        }
        else {
            chunkGrid.enabled = false;
        }
    }
}
