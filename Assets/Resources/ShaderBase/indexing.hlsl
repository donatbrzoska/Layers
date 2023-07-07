uint XYZ(uint x, uint y, uint z, uint3 dimensions)
{
    return z * dimensions.y * dimensions.x + y * dimensions.x + x;
}

uint XY(uint x, uint y, uint width)
{
    return y * width + x;
}