#include "basic_util.hlsl"
#include "indexing.hlsl"
#include "logging.hlsl"

int3 id__;
int2 SubgridGroupSize;
int2 SubgridCurrentThreadID;

int2 id()
{
    int2 subgrid_id = int2(id__.x, id__.y);
    return subgrid_id * SubgridGroupSize + SubgridCurrentThreadID;
}

int2 CalculationPosition;
int2 CalculationSize;

RWStructuredBuffer<float4> Debug;
RWStructuredBuffer<DebugListInfo> DebugInfo;

int DEBUG_LIST_SIZE()
{
    return 16;
}

void set_debug_list_info(int size, int t)
{
    DebugListInfo dli;
    dli.Size = size;
    dli.Type = t;
    DebugInfo[0] = dli;
}

void log_(int index, float4 f)
{
    Debug[XYZ(id().x, id().y, index, int3(CalculationSize.x, CalculationSize.y, DEBUG_LIST_SIZE()))] = f;
}