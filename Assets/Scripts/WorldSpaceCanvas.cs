using UnityEngine;

public class WorldSpaceCanvas
{
    public Vector2Int TextureSize { get; private set; }

    public Vector2 Size { get; private set;}
    public Vector3 Position { get; private set;}
    private int Resolution;

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

    public Vector2Int MapToPixel(Vector3 worldSpace)
    {
        float pixelWidthInWorldSpace = 1f/Resolution;
        Vector3 lowerLeftPixelCenter = new Vector2(XMin, YMin) + new Vector2(0.5f * pixelWidthInWorldSpace, 0.5f * pixelWidthInWorldSpace);
        float dx = worldSpace.x - lowerLeftPixelCenter.x;
        float dy = worldSpace.y - lowerLeftPixelCenter.y;

        float px = dx / Size.x;
        float py = dy / Size.y;

        int pixelX = MathUtil.RoundToInt(px * (TextureSize.x));
        int pixelY = MathUtil.RoundToInt(py * (TextureSize.y));

        return new Vector2Int(pixelX, pixelY);
    }

    public Vector2Int MapToPixelInRange(Vector3 worldSpace)
    {
        bool yTopOOB = worldSpace.y >= YMax;
        if (yTopOOB)
            worldSpace.y = YMax - 0.0001f;

        bool yBottomOOB = worldSpace.y < YMin;
        if (yBottomOOB)
            worldSpace.y = YMin;

        bool xLeftOOB = worldSpace.x < XMin;
        if (xLeftOOB)
            worldSpace.x = XMin;

        bool yRightOOB = worldSpace.x >= XMax;
        if (yRightOOB)
            worldSpace.x = XMax - 0.0001f;

        return MapToPixel(worldSpace);
    }
}
