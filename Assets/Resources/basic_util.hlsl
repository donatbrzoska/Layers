bool is_relevant_thread(uint2 id, uint2 calculation_size)
{
    return all(id < calculation_size);
}

float unzero(float f) {
    return f + 0.000001;;
}

bool pixel_in_array_range(int2 pixel, uint2 target_size)
{
    return pixel.x >= 0
        && pixel.x < (int) target_size.x
        && pixel.y >= 0
        && pixel.y < (int) target_size.y;
}