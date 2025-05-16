using UnityEngine;
using UnityEngine.UI;
using UnityUtilities;
using System.Linq;

public class MapPreview : MonoBehaviour {
    public enum DrawMode { HeightMap, FalloffMap, ColorMap, WaterMap, GroundMap, ForestMap, CustomMap };

    public RawImage textureRenderer;
    public DrawMode drawMode;
    [Range(0, 1)] public float customMinValue = 0;
    [Range(0, 1)] public float customMaxValue = 1;

    public bool autoUpdate;

    Color[] colorMap;
    HeightMap previewHeightMap;

#if UNITY_EDITOR
    void OnValidate() {
        if (customMinValue > customMaxValue) customMinValue = customMaxValue;
    }
#endif

    public void DrawMapInEditor(int mapWidth, int mapHeight, HeightMap heightMap, HeightMapSettings heightMapSettings) {
        previewHeightMap = heightMap;
        colorMap = HeightMapGenerator.GenerateColorMap(mapWidth, mapHeight, heightMap.values, heightMapSettings);;

        if (drawMode == DrawMode.HeightMap) {
            DrawTexture(TextureGenerator.TextureFromHeightMap(previewHeightMap.values, previewHeightMap.minValue, previewHeightMap.maxValue));
        }
        else if (drawMode == DrawMode.FalloffMap) {
            DrawTexture(TextureGenerator.TextureFromHeightMap(previewHeightMap.falloffMap, 0, 1));
        }
        else if (drawMode == DrawMode.ColorMap && colorMap != null) {
            DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
        else if (drawMode == DrawMode.WaterMap) {
            DrawTexture(TextureGenerator.TextureFromHeightMap(HeightMapGenerator.GetMapValuesInRange(previewHeightMap.values,
                heightMapSettings.GetTerrainTypeByTile(TileType.DeepWater).height, 
                heightMapSettings.GetTerrainTypeByTile(TileType.Water).height, maxInclusive: true, normalized: true),
                previewHeightMap.minValue, previewHeightMap.maxValue));
        }
        else if (drawMode == DrawMode.GroundMap) {
            DrawTexture(TextureGenerator.TextureFromHeightMap(HeightMapGenerator.GetMapValuesInRange(previewHeightMap.values,
                heightMapSettings.GetTerrainTypeByTile(TileType.Sand).height, 
                heightMapSettings.GetTerrainTypeByTile(TileType.Rock2).height, maxInclusive: true, normalized: true),
                previewHeightMap.minValue, previewHeightMap.maxValue));
        }
        else if (drawMode == DrawMode.ForestMap) {
            DrawTexture(TextureGenerator.TextureFromHeightMap(HeightMapGenerator.GetMapValuesInRange(previewHeightMap.values,
                heightMapSettings.GetTerrainTypeByTile(TileType.Grass).height, 
                heightMapSettings.GetTerrainTypeByTile(TileType.DeepGrass).height, minInclusive: true, maxInclusive: true, normalized: true),
                previewHeightMap.minValue, previewHeightMap.maxValue));
        }
        else if (drawMode == DrawMode.CustomMap) {
            DrawTexture(TextureGenerator.TextureFromHeightMap(HeightMapGenerator.GetMapValuesInRange(previewHeightMap.values,
                customMinValue, customMaxValue, maxInclusive: true, normalized: true),
                previewHeightMap.minValue, previewHeightMap.maxValue));
        }
        
        previewHeightMap.values = null;
        previewHeightMap.falloffMap = null;
        colorMap = null;
    }

    public void DrawTexture(Texture2D _texture) {
        // textureRenderer.sharedMaterial.mainTexture = texture;
        // textureRenderer.transform.localScale = new Vector3(texture.width / (width != 0 ? width : 1), 1, texture.height / (height != 0 ? height : 1));
        textureRenderer.texture = _texture;
    }

    // public void DrawTexture(Texture2D _texture, int width, int height) {
    //     // textureRenderer.sharedMaterial.mainTexture = texture;
    //     // textureRenderer.transform.localScale = new Vector3(texture.width / (width != 0 ? width : 1), 1, texture.height / (height != 0 ? height : 1));
    //     textureRenderer.texture = _texture;
    // }
}