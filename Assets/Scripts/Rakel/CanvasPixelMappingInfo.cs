using UnityEngine;

// Values for pixel to world space back conversion
public struct CanvasPixelMappingInfo {

    public CanvasPixelMappingInfo(
        Vector2Int calculationPosition,
        Vector2Int textureSize,
        Vector3 canvasPosition,
        Vector2 canvasSize,
        Vector3 rakelAnchor,
        Vector3 rakelPosition,
        float rakelLength,
        float rakelWidth,
        float rakelRotation,
        Vector2 rakelOriginBoundaries)
    {
        CalculationPosition = calculationPosition;

        TextureSize = textureSize;
        CanvasPosition = canvasPosition;
        CanvasSize = canvasSize;
        RakelAnchor = rakelAnchor;
        RakelPosition = rakelPosition;
        RakelLength = rakelLength;
        RakelWidth = rakelWidth;
        RakelRotation = rakelRotation;
        
        RakelOriginBoundaries = rakelOriginBoundaries;

        pad0 = 0;
        pad1 = 0;
        pad2 = 0;
        pad3 = 0;
    }

    public Vector3 CanvasPosition;
    public float pad0;
    public Vector3 RakelAnchor;
    public float pad1;
    public Vector3 RakelPosition;
    public float pad2;
    public Vector2Int CalculationPosition; // ... Lowest left pixel on canvas that is modified though this shader computation
    public Vector2Int TextureSize;
    public Vector2 CanvasSize;
    public Vector2 RakelOriginBoundaries; // Tilted rakel boundary description
    public float RakelLength;
    public float RakelWidth;
    public float RakelRotation; // TODO maybe use rounded boundaries for this angle
    public float pad3;

    // public Vector2Int CalculationPosition; // ... Lowest left pixel on canvas that is modified though this shader computation

    // public Vector2Int TextureSize;
    // public Vector3 CanvasPosition;
    // public Vector2 CanvasSize;

    // public Vector3 RakelAnchor;
    // public Vector3 RakelPosition;
    // public float RakelLength;
    // public float RakelWidth;

    // public float RakelRotation; // TODO maybe use rounded boundaries for this angle

    // public Vector2 RakelOriginBoundaries; // Tilted rakel boundary description
}