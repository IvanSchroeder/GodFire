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

    int TILING = Shader.PropertyToID("_Tiling");
    int TILING_OFFSET = Shader.PropertyToID("_TilingOffset");
    int POSITION_OFFSET = Shader.PropertyToID("_PositionOffset");

    int SNAP_TO_INT = Shader.PropertyToID("_SnapToInt");
    int CENTER_GRID = Shader.PropertyToID("_CenterGrid");
    int ISOMETRIC_GRID = Shader.PropertyToID("_IsometricGrid");
    int RENDER_GRID = Shader.PropertyToID("_RenderGrid");
    int CUTOFF_GRID = Shader.PropertyToID("_CutoffGrid");
    int GRID_ALPHA = Shader.PropertyToID("_GridAlpha");

    int PIXELS_PER_UNIT = Shader.PropertyToID("_PPU");
    int SQUARED_SIZED = Shader.PropertyToID("_SquaredSize");
    int GRID_SIZE = Shader.PropertyToID("_GridSize");
    int GRID_WIDTH = Shader.PropertyToID("_GridWidth");
    int GRID_HEIGHT = Shader.PropertyToID("_GridHeight");
    int ODD_GRID_OFFSET = Shader.PropertyToID("_OddGridOffset");

    int RENDER_CELLS = Shader.PropertyToID("_RenderCells");
    int CELLS_THICKNESS = Shader.PropertyToID("_CellsThickness");
    int CELLS_ALPHA = Shader.PropertyToID("_CellsAlpha");
    int CELLS_COLOR = Shader.PropertyToID("_CellsColor");

    int RENDER_LINES = Shader.PropertyToID("_RenderLines");
    int LINES_THICKNESS = Shader.PropertyToID("_LinesThickness");
    int LINES_ALPHA = Shader.PropertyToID("_LinesAlpha");
    int LINES_COLOR = Shader.PropertyToID("_LinesColor");

#if UNITY_EDITOR
    void OnValidate() {
        if (gridMaterial.IsNull()) gridMaterial = new Material(gridShader);
        gridRenderer.sharedMaterial = gridMaterial;
        GetShaderProperties();

        SetShaderProperties();
    }
#endif

    void Awake() {
        if (gridMaterial.IsNull()) gridMaterial = new Material(gridShader);
        gridRenderer.sharedMaterial = gridMaterial;
        GetShaderProperties();
    }

    void Update() {
        SetShaderProperties();
    }

    void GetShaderProperties() {
        TILING = Shader.PropertyToID("_Tiling");
        TILING_OFFSET = Shader.PropertyToID("_TilingOffset");
        POSITION_OFFSET = Shader.PropertyToID("_PositionOffset");

        SNAP_TO_INT = Shader.PropertyToID("_SnapToInt");
        CENTER_GRID = Shader.PropertyToID("_CenterGrid");
        ISOMETRIC_GRID = Shader.PropertyToID("_IsometricGrid");
        RENDER_GRID = Shader.PropertyToID("_RenderGrid");
        CUTOFF_GRID = Shader.PropertyToID("_CutoffGrid");
        GRID_ALPHA = Shader.PropertyToID("_GridAlpha");

        PIXELS_PER_UNIT = Shader.PropertyToID("_PPU");
        SQUARED_SIZED = Shader.PropertyToID("_SquaredSize");
        GRID_SIZE = Shader.PropertyToID("_GridSize");
        GRID_WIDTH = Shader.PropertyToID("_GridWidth");
        GRID_HEIGHT = Shader.PropertyToID("_GridHeight");
        ODD_GRID_OFFSET = Shader.PropertyToID("_OddGridOffset");

        RENDER_CELLS = Shader.PropertyToID("_RenderCells");
        CELLS_THICKNESS = Shader.PropertyToID("_CellsThickness");
        CELLS_ALPHA = Shader.PropertyToID("_CellsAlpha");
        CELLS_COLOR = Shader.PropertyToID("_CellsColor");

        RENDER_LINES = Shader.PropertyToID("_RenderLines");
        LINES_THICKNESS = Shader.PropertyToID("_LinesThickness");
        LINES_ALPHA = Shader.PropertyToID("_LinesAlpha");
        LINES_COLOR = Shader.PropertyToID("_LinesColor");
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
