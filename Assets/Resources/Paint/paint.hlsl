#include "rgb_ryb_sugita_takahashi.hlsl"
#include "rgb_ryb_leonard.hlsl"

float unzero(float f);
float FLOAT_PRECISION();

int PAINT_UNIT()
{
    return 1;
}

float VOLUME_THICKNESS()
{
    return 0.001; // unity worldspace, so 1 volume = 1 mm
}

struct Paint {
    float4 color;
    float volume;
};

Paint paint_create_empty()
{
    Paint p;
    p.color = float4(0,0,0,0);
    p.volume = 0;
    return p;
}

Paint paint_create(float4 color, float volume)
{
    Paint p;
    p.color = color;
    p.volume = volume;
    return p;
}

float4 rgb_to_ryb(float4 rgba)
{
    float3 rgb = float3(rgba.x, rgba.y, rgba.z);
    float3 ryb = rgb_to_ryb_st(rgb);
    // float3 ryb = rgb_to_ryb_leonard(rgb);
    float4 ryba = float4(ryb.x, ryb.y, ryb.z, 1);
    return ryba;
}

float4 ryb_to_rgb(float4 ryba)
{
    float3 ryb = float3(ryba.x, ryba.y, ryba.z);
    float3 rgb = ryb_to_rgb_st(ryb);
    // float3 rgb = ryb_to_rgb_leonard(ryb);
    float4 rgba = float4(rgb.x, rgb.y, rgb.z, 1);
    return rgba;
}

// It is assumed that part_a + part_b = 1
float4 mix_colors(float4 a, float a_part, float4 b, float b_part)
{
    float4 result = a_part * a + b_part * b;
    return float4(result.x, result.y, result.z, 1);
}

Paint mix(Paint a, Paint b)
{
    // TODO find real source of negative values
    // ensure >= 0 values
    a.volume = max(a.volume, 0);
    b.volume = max(b.volume, 0);

    Paint result = paint_create_empty();
    result.volume = a.volume + b.volume;

    if (result.volume > 0)
    {
        float a_part = a.volume / result.volume;
        float b_part = 1 - a_part;
        result.color = mix_colors(a.color, a_part, b.color, b_part);
    }

    return result;
}

bool is_empty(Paint p)
{
    return p.volume < FLOAT_PRECISION();
}

// p.volume is assumed to be 0..1
Paint alpha_blend(Paint p, float4 background_color, float paint_unit)
{
    float real_volume = p.volume;

    // 4th root makes a lot optical thickness with little volume
    p.volume = pow(abs(p.volume), (float)1/4);

    Paint canvas_paint;
    canvas_paint.color = background_color;
    canvas_paint.volume = max(paint_unit - p.volume, 0);

    Paint mixed = mix(canvas_paint, p);
    mixed.volume = real_volume;

    return mixed;
}