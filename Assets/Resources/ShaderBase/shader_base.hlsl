#include "basic_util.hlsl"
#include "indexing.hlsl"
#include "logging.hlsl"

uint2 subgrid_id;
uint2 SubgridGroupSize;
uint2 SubgridCurrentThreadID;

void set_subgrid_id(uint3 id_)
{
    subgrid_id = id_.xy;
}

uint2 id()
{
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