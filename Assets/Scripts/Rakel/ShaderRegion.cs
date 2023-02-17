using UnityEngine;

public class ShaderRegion
{
    public Vector2Int ThreadGroups;

    // Number of columns and rows the shader needs to be active in
    public Vector2Int CalculationSize;

    // Pixel coordinates on canvas of lower left pixel of calculation
    public Vector2Int CalculationPosition;

    // Given are four pixel coordinates on the canvas, the thread group sizes and an optional padding
    public ShaderRegion(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d, Vector2Int groupSize, int padding)
    {
        int minX = Mathf.Min(Mathf.Min(Mathf.Min(a.x, b.x), c.x), d.x) - padding;
        int maxX = Mathf.Max(Mathf.Max(Mathf.Max(a.x, b.x), c.x), d.x) + padding;
        int minY = Mathf.Min(Mathf.Min(Mathf.Min(a.y, b.y), c.y), d.y) - padding;
        int maxY = Mathf.Max(Mathf.Max(Mathf.Max(a.y, b.y), c.y), d.y) + padding;

        int dx = maxX - minX;
        int cols = dx + 1;
        ThreadGroups.x = cols / groupSize.x;
        if (ThreadGroups.x * groupSize.x < cols)
        {
            ThreadGroups.x++;
        }

        int dy = maxY - minY;
        int rows = dy + 1;
        ThreadGroups.y = rows / groupSize.y;
        if (ThreadGroups.y * groupSize.y < rows)
        {
            ThreadGroups.y++;
        }

        CalculationSize.x = maxX - minX + 1;
        CalculationSize.y = maxY - minY + 1;

        CalculationPosition.x = minX;
        CalculationPosition.y = minY;
    }

    public ShaderRegion(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d, Vector2Int groupSize) : this(a, b, c, d, groupSize, 0) { }
}