float unzero(float f);

float4 CANVAS_COLOR()
{
    return float4(1,1,1,1);
}

int PAINT_UNIT()
{
    return 1000;
}

struct Paint {
    float4 color;
    int volume;
};

Paint mix(Paint a, Paint b)
{
    float volume = a.volume + b.volume;
    float a_part = (float)a.volume / unzero(volume);
    float b_part = (float)b.volume / unzero(volume);
    Paint result;
    result.color = a_part * a.color + b_part * b.color;
    result.volume = volume;

    return result;
}

bool is_empty(Paint p)
{
    return p.volume <= 10;
}

Paint simulate_alpha(Paint p)
{
    Paint canvas_paint;
    canvas_paint.color = CANVAS_COLOR();
    canvas_paint.volume = max(PAINT_UNIT() - p.volume, 0);

    Paint mixed = mix(canvas_paint, p);
    mixed.volume = p.volume;

    return mixed;
}