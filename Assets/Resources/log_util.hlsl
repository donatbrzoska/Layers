void log_(float4 f);
void set_debug_type(int t);

void log_float(float f)
{
    log_(float4(f,0,0,0));
    set_debug_type(1);
}

void log_float2(float2 f)
{
    log_(float4(f.x,f.y,0,0));
    set_debug_type(2);
}

void log_float3(float3 f)
{
    log_(float4(f.x,f.y,f.z,0));
    set_debug_type(3);
}

void log_float4(float4 f)
{
    log_(f);
    set_debug_type(4);
}