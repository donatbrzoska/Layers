float3 rgb_to_ryb_st(float3 RGB)
{
    float I_w = min(min(RGB.x, RGB.y), RGB.z);
    float3 rgb = RGB - I_w;

    float3 ryb = float3(
        rgb.x - min(rgb.x, rgb.y),
        (rgb.y + min(rgb.x, rgb.y))/2,
        (rgb.z + rgb.y - min(rgb.x, rgb.y))/2
    );

    float n = (max(max(ryb.x, ryb.y), ryb.z)) / unzero((max(max(rgb.x, rgb.y), rgb.z)));
    float3 ryb_ = ryb/unzero(n);

    float I_b = min(min(1 - RGB.x, 1 - RGB.y), 1 - RGB.z);
    float3 RYB = ryb_ + I_b;
    return RYB;
}

float3 ryb_to_rgb_st(float3 RYB)
{
    float I_b = min(min(RYB.x, RYB.y), RYB.z);
    float3 ryb = RYB - I_b;

    float3 rgb = float3(
        ryb.x + ryb.y - min(ryb.y, ryb.z),
        ryb.y + 2 * min(ryb.y, ryb.z),
        2 * (ryb.z - min(ryb.y, ryb.z))
    );

    float n = (max(max(rgb.x, rgb.y), rgb.z)) / unzero((max(max(ryb.x, ryb.y), ryb.z)));
    float3 rgb_ = rgb/unzero(n);

    float I_w = min(min(1 - RYB.x, 1 - RYB.y), 1 - RYB.z);
    float3 RGB = rgb_ + I_w;
    return RGB;
}