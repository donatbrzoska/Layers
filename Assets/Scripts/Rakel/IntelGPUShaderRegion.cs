using System;
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

    public IntelGPUShaderRegion(WorldSpaceCanvas wsc, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        Vector2Int a_ = wsc.MapToPixelInRange(a);
        Vector2Int b_ = wsc.MapToPixelInRange(b);
        Vector2Int c_ = wsc.MapToPixelInRange(c);
        Vector2Int d_ = wsc.MapToPixelInRange(d);

        int minX = Mathf.Min(Mathf.Min(Mathf.Min(a_.x, b_.x), c_.x), d_.x);
        int maxX = Mathf.Max(Mathf.Max(Mathf.Max(a_.x, b_.x), c_.x), d_.x);
        int minY = Mathf.Min(Mathf.Min(Mathf.Min(a_.y, b_.y), c_.y), d_.y);
        int maxY = Mathf.Max(Mathf.Max(Mathf.Max(a_.y, b_.y), c_.y), d_.y);

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