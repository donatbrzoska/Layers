using System;
using UnityEngine;
public class IndexUtil
{
    public static int XY(int x, int y, int width)
    {
        return y * width + x;
    }

    public static int XYZ(int x, int y, int z, Vector3Int dim)
    {
        return z * dim.x * dim.y + y * dim.x + x;
    }
}
