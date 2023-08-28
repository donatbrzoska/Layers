using System;
using UnityEngine;
public class IndexUtil
{
    public static int XY(int x, int y, Vector2Int dim)
    {
        // return x * dim.y + y; // column-major
        return y * dim.x + x; // row-major
    }

    public static int XYZ(int x, int y, int z, Vector3Int dim)
    {
        // return z + y * dim.z + x * dim.y * dim.z; // stack by stack, column-major
        return z + x * dim.z + y * dim.x * dim.z; // stack by stack, row-major
        // return z * dim.y * dim.x + x * dim.y + y; // layer by layer, column-major
        //return z * dim.y * dim.x + y * dim.x + x; // layer by layer, row-major
    }
}
