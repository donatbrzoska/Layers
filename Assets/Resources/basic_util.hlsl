bool is_relevant_thread(uint3 id, uint2 calculation_size)
{
    return all(id.xy < calculation_size);
}

float unzero(float f) {
    return f + 0.000001;;
}