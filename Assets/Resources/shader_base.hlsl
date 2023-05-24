// ######################################## SHADER BASE ########################################

#pragma kernel main

#include "basic_util.hlsl"
#include "index_util.hlsl"
#include "log_util.hlsl"

uint3 id;

int2 CalculationPosition;
uint2 CalculationSize;

RWStructuredBuffer<float4> Debug;
RWStructuredBuffer<DebugListInfo> DebugInfo;

void set_debug_list_info(uint size, uint t)
{
    DebugListInfo dli;
    dli.Size = size;
    dli.Type = t;
    DebugInfo[0] = dli;
}

void log_(uint index, float4 f)
{
    Debug[XYZ(id.x, id.y, index, CalculationSize)] = f;
}

// ###################################### SHADER BASE END ######################################
