struct Paint {
    float4 color;
    int volume;
};

Paint mix(Paint a, Paint b)
{
    int volume = a.volume + b.volume + 0.000001; // prevent division by zero
    float a_part = (float)a.volume / (float)volume;
    float b_part = (float)b.volume / (float)volume;
    Paint result;
    result.color = a_part * a.color + b_part * b.color;
    result.volume = volume;

    return result;
}