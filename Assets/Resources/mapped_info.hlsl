struct MappedInfo {
    float3 transformed_pixel;
    float2 reservoir_pixel;
    float distance;
    float overlap[9][9];
    float total_overlap;
    float target_volume_to_transfer;
};