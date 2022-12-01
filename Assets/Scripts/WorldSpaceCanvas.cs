using UnityEngine;

public class WorldSpaceCanvas
{
    public int PixelsX { get; private set; }
    public int PixelsY { get; private set; }

    private float Height;
    private float Width;
    private Vector3 Position;
    private int Resolution;

    public float XMin { get; private set; }
    private float XMax;
    public float YMin { get; private set; }
    private float YMax;

    // It is assumed that the canvas is perpendicular to the z axis
    // Position is the center of the canvas
    public WorldSpaceCanvas(float height, float width, int resolution, Vector3 position)
    {
        Height = height;
        Width = width;
        Position = position;
        Resolution = resolution;

        XMin = position.x - width / 2;
        XMax = position.x + width / 2;
        YMin = position.y - height / 2;
        YMax = position.y + height / 2;

        PixelsX = (int)(Resolution * width);
        PixelsY = (int)(Resolution * height);
    }

    public Vector2Int MapToPixel(Vector3 worldSpace)
    {
        Vector3 lowerLeft = new Vector2(XMin, YMin);
        float dx = worldSpace.x - lowerLeft.x;
        float dy = worldSpace.y - lowerLeft.y;

        float px = dx / Width;
        float py = dy / Height;

        // out of range
        if (px > 1 || px < 0 || py < 0 || py > 1)
        {
            return new Vector2Int(int.MinValue, int.MinValue);
        }

        int pixelWidth = (int)(Resolution * Width);
        int pixelHeight = (int)(Resolution * Height);

        int pixelX = MathUtil.RoundToInt(px * (pixelWidth - 1));
        int pixelY = MathUtil.RoundToInt(py * (pixelHeight - 1));

        return new Vector2Int(pixelX, pixelY);
    }

    public Vector2Int MapToPixelInRange(Vector3 worldSpace)
    {
        bool yTopOOB = worldSpace.y > YMax;
        if (yTopOOB)
            worldSpace.y = YMax;

        bool yBottomOOB = worldSpace.y < YMin;
        if (yBottomOOB)
            worldSpace.y = YMin;

        bool xLeftOOB = worldSpace.x < XMin;
        if (xLeftOOB)
            worldSpace.x = XMin;

        bool yRightOOB = worldSpace.x > XMax;
        if (yRightOOB)
            worldSpace.x = XMax;

        return MapToPixel(worldSpace);
    }
}
