using System;
public class IndexUtil
{
    public static int XY(int x, int y, int width)
    {
        return y * width + x;
    }

    public static int XYZ(int x, int y, int z, int width, int height)
    {
        return z * width * height + y * width + x;
    }
}
