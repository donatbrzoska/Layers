// void log_float_at(uint index, float f);

float distance_from_rakel(float3 point_pos, float3 rakel_LL_tilted, float3 rakel_LR_tilted, float3 rakel_position_tilted)
{
    float m = (rakel_LR_tilted.z - rakel_LL_tilted.z) / (rakel_LR_tilted.x - rakel_LL_tilted.x);
    float c = rakel_position_tilted.z - m * rakel_position_tilted.x;
    float h = m * point_pos.x + c;
    return abs(h - point_pos.z);
}

float distance_from_canvas(float3 point_pos, float3 canvas_position, float3 canvas_normal)
{
    float3 normal_0 = normalize(canvas_normal);
    return abs(dot(point_pos - canvas_position, normal_0));
}