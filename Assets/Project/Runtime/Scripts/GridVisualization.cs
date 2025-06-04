using UnityEngine;
using UnityUtilities;

public class GridVisualization : MonoBehaviour {
    [Header("References")]
    [Space(5f)]

    [SerializeField] GameObject visualizationObject;
    [SerializeField] SpriteRenderer gridRenderer;
    [SerializeField] Material gridMaterial;
    [SerializeField] Shader gridShader;

    [Space(10f)]
    [Header("Grid Parameters")]
    [Space(5f)]
    [SerializeField] float tiling;
    [SerializeField] Vector2 tilingOffset;
    [SerializeField] Vector2 positionOffset;

    [Space(2f)]

    [SerializeField] bool snapToInt;
    [SerializeField] bool centerGrid;
    [SerializeField] bool isometricGrid;
    [SerializeField] bool renderGrid;
    [SerializeField] bool cutoffGrid;
    [SerializeField] float gridAlpha;

    [Space(2f)]

    [SerializeField] int pixelsPerUnit;
    [SerializeField] bool squaredSized;
    [SerializeField] float gridSize;
    [SerializeField] float gridWidth;
    [SerializeField] float gridHeight;
    [SerializeField] float oddGridOffset;

    [Space(10f)]
    [Header("Cells Parameters")]
    [Space(5f)]
    [SerializeField] bool renderCells;
    [SerializeField] float cellsThickness;
    [SerializeField] Color cellsColor;
    [SerializeField] float cellsAlpha;

    [Space(10f)]
    [Header("Lines Parameters")]
    [Space(5f)]
    [SerializeField] bool renderLines;
    [SerializeField] float linesThickness;
    [SerializeField] Color linesColor;
    [SerializeField] float linesAlpha;

    int TILING = Shader.PropertyToID("Tiling");
    int TILING_OFFSET = Shader.PropertyToID("TilingOffset");
    int POSITION_OFFSET = Shader.PropertyToID("PositionOffset");

    int SNAP_TO_INT = Shader.PropertyToID("SnapToInt");
    int CENTER_GRID = Shader.PropertyToID("CenterGrid");
    int ISOMETRIC_GRID = Shader.PropertyToID("IsometricGrid");
    int RENDER_GRID = Shader.PropertyToID("RenderGrid");
    int CUTOFF_GRID = Shader.PropertyToID("CutoffGrid");
    int GRID_ALPHA = Shader.PropertyToID("GridAlpha");

    int PIXELS_PER_UNIT = Shader.PropertyToID("PPU");
    int SQUARED_SIZED = Shader.PropertyToID("SquaredSize");
    int GRID_SIZE = Shader.PropertyToID("GridSize");
    int GRID_WIDTH = Shader.PropertyToID("GridWidth");
    int GRID_HEIGHT = Shader.PropertyToID("GridHeight");
    int ODD_GRID_OFFSET = Shader.PropertyToID("OddGridOffset");

    int RENDER_CELLS = Shader.PropertyToID("RenderCells");
    int CELLS_THICKNESS = Shader.PropertyToID("CellsThickness");
    int CELLS_ALPHA = Shader.PropertyToID("CellsAlpha");
    int CELLS_COLOR = Shader.PropertyToID("CellsColor");

    int RENDER_LINES = Shader.PropertyToID("RenderLines");
    int LINES_THICKNESS = Shader.PropertyToID("LinesThickness");
    int LINES_ALPHA = Shader.PropertyToID("LinesAlpha");
    int LINES_COLOR = Shader.PropertyToID("LinesColor");

    void OnValidate() {
        if (gridMaterial.IsNull()) gridMaterial = new Material(gridShader);
        gridRenderer.sharedMaterial = gridMaterial;
        GetShaderProperties();

        SetShaderProperties();
    }

    void Awake() {
        if (gridMaterial.IsNull()) gridMaterial = new Material(gridShader);
        gridRenderer.sharedMaterial = gridMaterial;
        GetShaderProperties();
    }

    void Update() {
        SetShaderProperties();
    }

    void GetShaderProperties() {
        TILING = Shader.PropertyToID("Tiling");
        TILING_OFFSET = Shader.PropertyToID("TilingOffset");
        POSITION_OFFSET = Shader.PropertyToID("PositionOffset");

        SNAP_TO_INT = Shader.PropertyToID("SnapToInt");
        CENTER_GRID = Shader.PropertyToID("CenterGrid");
        ISOMETRIC_GRID = Shader.PropertyToID("IsometricGrid");
        RENDER_GRID = Shader.PropertyToID("RenderGrid");
        CUTOFF_GRID = Shader.PropertyToID("CutoffGrid");
        GRID_ALPHA = Shader.PropertyToID("GridAlpha");

        PIXELS_PER_UNIT = Shader.PropertyToID("PPU");
        SQUARED_SIZED = Shader.PropertyToID("SquaredSize");
        GRID_SIZE = Shader.PropertyToID("GridSize");
        GRID_WIDTH = Shader.PropertyToID("GridWidth");
        GRID_HEIGHT = Shader.PropertyToID("GridHeight");
        ODD_GRID_OFFSET = Shader.PropertyToID("OddGridOffset");

        RENDER_CELLS = Shader.PropertyToID("RenderCells");
        CELLS_THICKNESS = Shader.PropertyToID("CellsThickness");
        CELLS_ALPHA = Shader.PropertyToID("CellsAlpha");
        CELLS_COLOR = Shader.PropertyToID("CellsColor");

        RENDER_LINES = Shader.PropertyToID("RenderLines");
        LINES_THICKNESS = Shader.PropertyToID("LinesThickness");
        LINES_ALPHA = Shader.PropertyToID("LinesAlpha");
        LINES_COLOR = Shader.PropertyToID("LinesColor");
    }

    void SetShaderProperties() {
        gridRenderer.sharedMaterial.SetFloat(TILING, tiling);
        gridRenderer.sharedMaterial.SetVector(TILING_OFFSET, tilingOffset);
        gridRenderer.sharedMaterial.SetVector(POSITION_OFFSET, positionOffset);

        gridRenderer.sharedMaterial.SetInt(SNAP_TO_INT, snapToInt.ToInt());
        gridRenderer.sharedMaterial.SetInt(CENTER_GRID, centerGrid.ToInt());
        gridRenderer.sharedMaterial.SetInt(ISOMETRIC_GRID, isometricGrid.ToInt());
        gridRenderer.sharedMaterial.SetInt(RENDER_GRID, renderGrid.ToInt());
        gridRenderer.sharedMaterial.SetInt(CUTOFF_GRID, cutoffGrid.ToInt());
        gridRenderer.sharedMaterial.SetFloat(GRID_ALPHA, gridAlpha);

        gridRenderer.sharedMaterial.SetFloat(PIXELS_PER_UNIT, pixelsPerUnit);
        gridRenderer.sharedMaterial.SetInt(SQUARED_SIZED, squaredSized.ToInt());
        gridRenderer.sharedMaterial.SetFloat(GRID_SIZE, gridSize);
        gridRenderer.sharedMaterial.SetFloat(GRID_WIDTH, gridWidth);
        gridRenderer.sharedMaterial.SetFloat(GRID_HEIGHT, gridHeight);
        gridRenderer.sharedMaterial.SetFloat(ODD_GRID_OFFSET, oddGridOffset);

        gridRenderer.sharedMaterial.SetInt(RENDER_CELLS, renderCells.ToInt());
        gridRenderer.sharedMaterial.SetFloat(CELLS_THICKNESS, cellsThickness);
        gridRenderer.sharedMaterial.SetColor(CELLS_COLOR, cellsColor);
        gridRenderer.sharedMaterial.SetFloat(CELLS_ALPHA, cellsAlpha);

        gridRenderer.sharedMaterial.SetInt(RENDER_LINES, renderLines.ToInt());
        gridRenderer.sharedMaterial.SetFloat(LINES_THICKNESS, linesThickness);
        gridRenderer.sharedMaterial.SetColor(LINES_COLOR, linesColor);
        gridRenderer.sharedMaterial.SetFloat(LINES_ALPHA, linesAlpha);

        gridRenderer.transform.localScale = squaredSized ? new Vector3(gridSize, gridSize / 2, 1) : new Vector3(gridWidth, gridHeight / 2, 1);
    }
}
