using UnityEngine;

public class ShaderRegion
{
    // Number of columns and rows the shader needs to be active in
    public Vector2Int Size;

    // Pixel coordinates on canvas of lower left pixel of calculation
    public Vector2Int Position;

    public int PixelCount { get { return Size.x * Size.y; } }

    public ShaderRegion(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d, int padding)
    {
        int minX = Mathf.Min(Mathf.Min(Mathf.Min(a.x, b.x), c.x), d.x) - padding;
        int maxX = Mathf.Max(Mathf.Max(Mathf.Max(a.x, b.x), c.x), d.x) + padding;
        int minY = Mathf.Min(Mathf.Min(Mathf.Min(a.y, b.y), c.y), d.y) - padding;
        int maxY = Mathf.Max(Mathf.Max(Mathf.Max(a.y, b.y), c.y), d.y) + padding;

        Size.x = maxX - minX + 1;
        Size.y = maxY - minY + 1;

        Position.x = minX;
        Position.y = minY;
    }

    public ShaderRegion(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d) : this(a, b, c, d, 0) { }

    public ShaderRegion(Vector2Int reduceRegionPosition, Vector2Int reduceRegionSize, Vector2Int reduceBlockSize)
    {
        Position = reduceRegionPosition;

        Vector2Int reduceRegionSizePadded = new Vector2Int(
            (int) Mathf.Ceil(((float) reduceRegionSize.x) / reduceBlockSize.x) * reduceBlockSize.x,
            (int) Mathf.Ceil(((float)reduceRegionSize.y) / reduceBlockSize.y) * reduceBlockSize.y);
        Size = new Vector2Int(
            reduceRegionSizePadded.x / reduceBlockSize.x,
            reduceRegionSizePadded.y / reduceBlockSize.y);
    }

    public override string ToString()
    {
        return base.ToString() + " at " + Position + " with dimensions " + Size;
    }
}
