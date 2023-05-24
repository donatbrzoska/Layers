struct DebugListInfo
{
    uint Size;
    uint Type;
};

void set_debug_list_info(uint size, uint t);

void log_(uint index, float4 f);


// LOG LISTS
// It is assumed that every logging thread is logging the same amount of elements!

void log_float_at(uint index, float f)
{
    log_(index, float4(f,0,0,0));
    set_debug_list_info(index+1, 1);
}

void log_float2_at(uint index, float2 f)
{
    log_(index, float4(f.x,f.y,0,0));
    set_debug_list_info(index+1, 2);
}

void log_float3_at(uint index, float3 f)
{
    log_(index, float4(f.x,f.y,f.z,0));
    set_debug_list_info(index+1, 3);
}

void log_float4_at(uint index, float4 f)
{
    log_(index, float4(f.x,f.y,f.z,f.w));
    set_debug_list_info(index+1, 4);
}


// LOG SINGLE VALUES

void log_float(float f)
{
    log_float_at(0, f);
}

void log_float2(float2 f)
{
    log_float2_at(0, f);
}

void log_float3(float3 f)
{
    log_float3_at(0, f);
}

void log_float4(float4 f)
{
    log_float4_at(0, f);
}