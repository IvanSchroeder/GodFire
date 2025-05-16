using UnityEngine;

public class GridVisualization : MonoBehaviour {
    [Header("References")]
    [Space(5f)]

    [SerializeField] GameObject visualizationObject;
    [SerializeField] SpriteRenderer gridRenderer;

    [Space(10f)]
    [Header("Grid Parameters")]
    [Space(5f)]
    float tiling;
    Vector2 localOffset;
    Vector2 globalOffset;
    bool snapToGrid;
    int pixelsPerUnit;
    bool squareSized;
    float gridSquaredSize;
    float gridSizeX;
    float gridSizeY;
    bool centerGrid;
    bool renderGrid;
    bool cuttoffGrid;
    float gridAlpha;

    [Space(10f)]
    [Header("Cells Parameters")]
    [Space(5f)]
    bool renderCells;
    float cellThickness;
    float cellAlpha;
    Color cellColor;

    [Space(10f)]
    [Header("Lines Parameters")]
    [Space(5f)]
    bool renderLines;
    float lineThickness;
    float lineAlpha;
    Color lineColor;

    void OnValidate() {
        
    }

    void Start() {
        
    }

    void Update() {
        
    }
}
