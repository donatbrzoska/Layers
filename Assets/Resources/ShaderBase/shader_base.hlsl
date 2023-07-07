#include "basic_util.hlsl"
#include "indexing.hlsl"
#include "logging.hlsl"

uint3 id__;
uint2 SubgridGroupSize;
uint2 SubgridCurrentThreadID;

uint2 id()
{
    uint2 subgrid_id = uint2(id__.x, id__.y);
    return subgrid_id * SubgridGroupSize + SubgridCurrentThreadID;
}

int2 CalculationPosition;
uint2 CalculationSize;

RWStructuredBuffer<float4> Debug;
RWStructuredBuffer<DebugListInfo> DebugInfo;

uint DEBUG_LIST_SIZE()
{
    return 16;
}

void set_debug_list_info(uint size, uint t)
{
    DebugListInfo dli;
    dli.Size = size;
    dli.Type = t;
    DebugInfo[0] = dli;
}

void log_(uint index, float4 f)
{
    Debug[XYZ(id().x, id().y, index, uint3(CalculationSize.x, CalculationSize.y, DEBUG_LIST_SIZE()))] = f;
}