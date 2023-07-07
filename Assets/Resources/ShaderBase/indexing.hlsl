int XYZ(int x, int y, int z, int3 dimensions)
{
    return z * dimensions.y * dimensions.x + y * dimensions.x + x;
}

int XY(int x, int y, int width)
{
    return y * width + x;
}