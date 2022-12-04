using UnityEngine;

public class IntelGPUShaderRegion
{
    private const int GROUP_SIZE = 8;

    public Vector2Int ThreadGroups;

    // Number of columns and rows the shader needs to be active in
    public Vector2Int CalculationSize;

    // Pixel coordinates on canvas of lower left pixel of calculation
    public Vector2Int CalculationPosition;

    public IntelGPUShaderRegion(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d)
    {
        int minX = Mathf.Min(Mathf.Min(Mathf.Min(a.x, b.x), c.x), d.x);
        int maxX = Mathf.Max(Mathf.Max(Mathf.Max(a.x, b.x), c.x), d.x);
        int minY = Mathf.Min(Mathf.Min(Mathf.Min(a.y, b.y), c.y), d.y);
        int maxY = Mathf.Max(Mathf.Max(Mathf.Max(a.y, b.y), c.y), d.y);

        int dx = maxX - minX;
        int cols = dx + 1;
        ThreadGroups.x = cols;

        int dy = maxY - minY;
        int rows = dy + 1;

        ThreadGroups.y = rows / GROUP_SIZE;
        if (rows % GROUP_SIZE > 0)
        {
            ThreadGroups.y++;
        }

        CalculationSize.x = maxX - minX + 1;
        CalculationSize.y = maxY - minY + 1;

        CalculationPosition.x = minX;
        CalculationPosition.y = minY;
    }
}