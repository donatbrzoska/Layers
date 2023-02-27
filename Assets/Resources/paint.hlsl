float unzero(float f);

float4 EMPTY_PAINT_COLOR()
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