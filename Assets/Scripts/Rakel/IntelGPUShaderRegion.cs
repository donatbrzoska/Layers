using UnityEngine;

public class IntelGPUShaderRegion
{
    private const int GROUP_SIZE = 8;

    public int ThreadGroupsX { get; private set; }
    public int ThreadGroupsY { get; private set; }

    // Number of columns and rows the shader needs to be active in
    public int CalculationSizeX { get; private set; }
    public int CalculationSizeY { get; private set; }

    // Pixel coordinates on canvas of lower left pixel of calculation
    public int CalculationPositionX { get; private set; }
    public int CalculationPositionY { get; private set; }

    public IntelGPUShaderRegion(Vector2Int a, Vector2Int b, Vector2Int c, Vector2Int d)
    {
        int minX = Mathf.Min(Mathf.Min(Mathf.Min(a.x, b.x), c.x), d.x);
        int maxX = Mathf.Max(Mathf.Max(Mathf.Max(a.x, b.x), c.x), d.x);
        int minY = Mathf.Min(Mathf.Min(Mathf.Min(a.y, b.y), c.y), d.y);
        int maxY = Mathf.Max(Mathf.Max(Mathf.Max(a.y, b.y), c.y), d.y);

        int dx = maxX - minX;
        int cols = dx + 1;
        ThreadGroupsX = cols;

        int dy = maxY - minY;
        int rows = dy + 1;

        ThreadGroupsY = rows / GROUP_SIZE;
        if (rows % GROUP_SIZE > 0)
        {
            ThreadGroupsY++;
        }

        CalculationSizeX = maxX - minX + 1;
        CalculationSizeY = maxY - minY + 1;

        CalculationPositionX = minX;
        CalculationPositionY = minY;
    }
}