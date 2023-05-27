// void log_float_at(uint index, float f);

float distance_from_rakel(float3 point_pos, float3 rakel_LL_tilted, float3 rakel_LR_tilted, float3 rakel_position)
{
    float m = (rakel_LR_tilted.z - rakel_LL_tilted.z) / (rakel_LR_tilted.x - rakel_LL_tilted.x);
    float c = rakel_position.z - m * rakel_position.x;
    float h = m * point_pos.x + c;
    return h - point_pos.z;
}