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
    return 0.01; // 1 volume = 1mm = 0.01dm = 0.01 unity worldspace
}

struct Paint {
    float3 color;
    float volume;
};

Paint paint_create_empty()
{
    Paint p;
    p.color = float3(0,0,0);
    p.volume = 0;
    return p;
}

Paint paint_create(float3 color, float volume)
{
    Paint p;
    p.color = color;
    p.volume = volume;
    return p;
}

float3 rgb_to_ryb(float3 rgba)
{
    float3 rgb = float3(rgba.x, rgba.y, rgba.z);
    float3 ryb = rgb_to_ryb_st(rgb);
    // float3 ryb = rgb_to_ryb_leonard(rgb);
    return ryb;
}

float3 ryb_to_rgb(float3 ryba)
{
    float3 ryb = float3(ryba.x, ryba.y, ryba.z);
    float3 rgb = ryb_to_rgb_st(ryb);
    // float3 rgb = ryb_to_rgb_leonard(ryb);
    return rgb;
}

// It is assumed that part_a + part_b = 1
float3 mix_colors(float3 a, float a_part, float3 b, float b_part)
{
    return a_part * a + b_part * b;
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
Paint alpha_blend(Paint p, float3 background_color)
{
    float real_volume = p.volume;

    // 4th root makes a lot optical thickness with little volume
    p.volume = pow(abs(p.volume), (float)1/4);

    Paint canvas_paint;
    canvas_paint.color = background_color;
    canvas_paint.volume = max(PAINT_UNIT() - p.volume, 0);

    Paint mixed = mix(canvas_paint, p);
    mixed.volume = real_volume;

    return mixed;
}