using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapExtensions {
    /// <summary>
    /// Get the transform components for a tile. Convenience Function.
    /// </summary>
    /// <param name="map">Tilemap</param>
    /// <param name="position">position on map</param>
    /// <param name="tPosition">transform's position placed here</param>
    /// <param name="tRotation">transform's rotation placed here</param>
    /// <param name="tScale">transform's scale placed here</param>
    /// <remarks>Handy for tweening the transform (pos,scale,rot) of a tile</remarks>
    /// <remarks>No checking for whether or not a tile exists at that position</remarks>
    public static void GetTransformComponents(this Tilemap map, Vector3Int position, out Vector3 tPosition, out Vector3 tRotation, out Vector3 tScale) {
        var transform = map.GetTransformMatrix(position);
        tPosition = transform.GetColumn(3);
        tRotation = transform.rotation.eulerAngles;
        tScale = transform.lossyScale;
    }

    /// <summary>
    /// Set the transform for a tile. Convenience function.
    /// </summary>
    /// <param name="map">tilemap</param>
    /// <param name="position">position on map</param>
    /// <param name="tPosition">position for the tile transform</param>
    /// <param name="tRotation">rotation for the tile transform</param>
    /// <param name="tScale">scale for the tile transform</param>
    /// <remarks>Handy for tweening the transform (pos,scale,rot) of a tile's sprite</remarks>
    /// <remarks>No checking for whether or not a tile exists at that position</remarks>
    public static void SetTransform(this Tilemap map, Vector3Int position, Vector3 tPosition, Vector3 tRotation, Vector3 tScale) {
        map.SetTransformMatrix(position, Matrix4x4.TRS(tPosition, Quaternion.Euler(tRotation), tScale));
    }
}