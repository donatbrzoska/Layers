uint XYZ(uint x, uint y, uint z, uint3 dim)
{
    return z * dim.y * dim.x + y * dim.x + x;
}

uint XY(uint x, uint y, uint width)
{
    return y * width + x;
}