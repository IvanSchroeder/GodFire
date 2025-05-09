using UnityEngine;
using UnityEngine.UI;
using UnityUtilities;

public class MapPreview : MonoBehaviour {
    public enum DrawMode { HeightMap, FalloffMap, ColorMap };

    // public Renderer textureRenderer;
    // public Texture2D textureRenderer;
    public RawImage textureRenderer;
    public DrawMode drawMode;

    public bool autoUpdate;

    Color[] colorMap = new Color[0];

    public void DrawMapInEditor(HeightMap heightMap, int mapWidth, int mapHeight, Color[] _colorMap) {
        colorMap = _colorMap;

        if (drawMode == DrawMode.HeightMap) {
            DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap.values, heightMap.minValue, heightMap.maxValue));
        }
        else if (drawMode == DrawMode.FalloffMap) {
            DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap.falloffMap, 0, 1));
        }
        else if (drawMode == DrawMode.ColorMap) {
            DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
        
        // heightMap.values = null;
        // heightMap.falloffMap = null;
        // heightMap.colorMap = null;
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

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color color;
    public WorldTile worldTile;
}