using UnityEngine;

public class WorldSpaceCanvas
{
    public Vector2Int TextureSize { get; private set; }

    public Vector2 Size { get; private set;}
    public Vector3 Position { get; private set;}
    public int Resolution { get; private set; }

    public float XMin { get; private set; }
    private float XMax;
    public float YMin { get; private set; }
    private float YMax;

    // It is assumed that the canvas is perpendicular to the z axis
    // Position is the center of the canvas
    public WorldSpaceCanvas(float height, float width, int resolution, Vector3 position)
    {
        Size = new Vector2(width, height);
        Position = position;
        Resolution = resolution;

        XMin = position.x - width / 2;
        XMax = position.x + width / 2;
        YMin = position.y - height / 2;
        YMax = position.y + height / 2;

        TextureSize = new Vector2Int(
            (int)(Resolution*Size.x),
            (int)(Resolution*Size.y)
        );
    }

    public Vector3 AlignToPixelGrid(Vector3 point)
    {
        Vector2Int pixel = MapToPixel(point);
        Vector3 gridAlignedIncomplete = MapToWorldSpace(pixel);
        gridAlignedIncomplete.z = point.z;
        return gridAlignedIncomplete;
    }

    private Vector3 MapToWorldSpace(Vector2Int pixel)
    {
        float pixelSize = 1 / (float)Resolution;
        Vector3 positiveCanvasAligned = new Vector3(0.5f * pixelSize + pixel.x * pixelSize,
                                                    0.5f * pixelSize + pixel.y * pixelSize,
                                                    0);

        Vector3 canvasLowerLeft = Position - new Vector3(Size.x / 2, Size.y / 2, 0);
        Vector3 canvasAligned = positiveCanvasAligned + canvasLowerLeft;

        return canvasAligned;
    }

    public Vector2Int MapToPixel(Vector3 worldSpace)
    {
        Vector3 lowerLeftOriented = worldSpace + new Vector3(Size.x / 2, Size.y / 2, 0);
        // really lowerLeftOriented / pixelSize, but that is lowerLeftOriented / (1/Resolution)
        Vector2 floatPixel = lowerLeftOriented * Resolution;
        return new Vector2Int((int)Mathf.Ceil(floatPixel.x), (int)Mathf.Ceil(floatPixel.y)) - new Vector2Int(1, 1);
    }

    public Vector2Int MapToPixelInRange(Vector3 worldSpace)
    {
        bool yTopOOB = worldSpace.y > YMax;
        if (yTopOOB)
            worldSpace.y = YMax;

        bool yBottomOOB = worldSpace.y <= YMin;
        if (yBottomOOB)
            worldSpace.y = YMin + 0.0001f;

        bool yRightOOB = worldSpace.x > XMax;
        if (yRightOOB)
            worldSpace.x = XMax;

        bool xLeftOOB = worldSpace.x <= XMin;
        if (xLeftOOB)
            worldSpace.x = XMin + 0.0001f;

        return MapToPixel(worldSpace);
    }
}
