struct MappedInfo {
    float3 transformed_pixel;
    float2 reservoir_pixel;
    float distance;
    float overlap[9][9];
    float volume_to_transfer;
};