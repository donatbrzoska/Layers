uint XYZ(uint x, uint y, uint z, uint3 dim)
{
    // return z + y * dim.z + x * dim.y * dim.z; // stack by stack, column-major
    return z + x * dim.z + y * dim.x * dim.z; // stack by stack, row-major
    // return z * dim.y * dim.x + x * dim.y + y; // layer by layer, column-major
    // return z * dim.y * dim.x + y * dim.x + x; // layer by layer, row-major
}

uint XY(uint x, uint y, uint2 dim)
{
    // return x * dim.y + y; // column-major
    return y * dim.x + x; // row-major
}