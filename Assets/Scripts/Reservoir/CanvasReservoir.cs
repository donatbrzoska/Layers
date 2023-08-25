using UnityEngine;
using System.Collections.Generic;

public enum ReduceFunction
{
    Max,
    Avg
}

public enum InternalReduceFunction
{
    Max,
    Add,
}

public class CanvasReservoir : Reservoir
{

    // used for canvas snapshot buffer (CSB)
    // Paper: Detail-Preserving Paint Modeling for 3D Brushes (Chu et al., 2010)
    public PaintGrid PaintGridSnapshot;
    public PaintGrid PaintGridSnapshotImprintCopy; // only read from for sampling

    // used for auto z
    public ComputeBuffer PaintGridInfoSnapshot; // technically unnecessary, when CSB is enabled (without delete!)
    public ComputeBuffer Workspace;

    public CanvasReservoir(int resolution, int width, int height, int layers, float cellVolume)
        : base(resolution, width, height, layers, cellVolume)
    {
        PaintGridSnapshot = new PaintGrid(Size, cellVolume);
        PaintGridSnapshotImprintCopy = new PaintGrid(Size, cellVolume);
        PaintGridInfoSnapshot = new ComputeBuffer(Size.x * Size.y, ColumnInfo.SizeInBytes);
        Workspace = new ComputeBuffer(Size.x * Size.y, sizeof(float));
    }

    public void DoSnapshot(bool debugEnabled = false)
    {
        DoSnapshot(GetFullShaderRegion(), debugEnabled);
    }

    public void DoSnapshot(ShaderRegion sr, bool debugEnabled = false)
    {
        Duplicate(PaintGrid, PaintGridSnapshot, sr, debugEnabled);
    }

    // only copies pixels that are currently not under the rakel
    public void DoSnapshotUpdate(
        ComputeBuffer rakelMappedInfo,
        Vector2Int rakelMappedInfoSize,
        Vector3Int RakelReservoirSize,
        ShaderRegion shaderRegion,
        bool debugEnabled = false)
    {
        new ComputeShaderTask(
            "Reservoir/CopyInactive",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),
                new CSInt2("RakelMappedInfoSize", rakelMappedInfoSize),
                new CSInt3("RakelReservoirSize", RakelReservoirSize),

                new CSComputeBuffer("ReservoirInfo", PaintGrid.Info),
                new CSComputeBuffer("ReservoirContent", PaintGrid.Content),
                new CSComputeBuffer("ReservoirInfoDuplicate", PaintGridSnapshot.Info),
                new CSComputeBuffer("ReservoirContentDuplicate", PaintGridSnapshot.Content),
                new CSInt3("ReservoirSize", Size),
            },
            debugEnabled
        ).Run();
    }

    public void DoSnapshotImprintCopy(bool debugEnabled = false)
    {
        DoSnapshotImprintCopy(GetFullShaderRegion(), debugEnabled);
    }

    public void DoSnapshotImprintCopy(ShaderRegion sr, bool debugEnabled = false)
    {
        Duplicate(PaintGridSnapshot, PaintGridSnapshotImprintCopy, sr, debugEnabled);
    }

    public void SnapshotInfo(bool debugEnabled = false)
    {
        new ComputeShaderTask(
            "Reservoir/SnapshotInfo",
            GetFullShaderRegion(),
            new List<CSAttribute>()
            {
                new CSComputeBuffer("ReservoirInfo", PaintGrid.Info),
                new CSComputeBuffer("ReservoirInfoSnapshot", PaintGridInfoSnapshot),
                new CSInt3("ReservoirSize", Size),
            },
            debugEnabled
        ).Run();
    }

    public void CopyVolumesToWorkspace(bool debugEnabled = false)
    {
        CopyVolumesToWorkspace(PaintGrid.Info, GetFullShaderRegion(), debugEnabled);
    }

    public void CopyVolumesToWorkspace(ComputeBuffer sourceInfo, ShaderRegion sr, bool debugEnabled = false)
    {
        new ComputeShaderTask(
            "Reservoir/CopyVolumesToWorkspace",
            sr,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("ReservoirInfo", sourceInfo),
                new CSComputeBuffer("Workspace", Workspace),
                new CSInt3("ReservoirSize", Size)
            },
            debugEnabled
        ).Run();
    }

    public void CopySnapshotActiveInfoVolumesToWorkspace(
        ComputeBuffer rakelMappedInfo,
        Vector2Int rakelMappedInfoSize,
        Vector2Int rakelReservoirSize,
        ShaderRegion shaderRegion,
        bool debugEnabled = false)
    {
        new ComputeShaderTask(
            "Reservoir/CopyActiveInfoVolumesToWorkspace",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),
                new CSInt2("RakelMappedInfoSize", rakelMappedInfoSize),
                new CSInt2("RakelReservoirSize", rakelReservoirSize),

                new CSComputeBuffer("ReservoirInfo", PaintGridInfoSnapshot),
                new CSComputeBuffer("Workspace", Workspace),
                new CSInt3("ReservoirSize", Size)
            },
            debugEnabled
        ).Run();
    }

    // NOTE: It is assumed that the data is copied to workspace already
    public void ReduceActiveWorkspace(
        ComputeBuffer rakelMappedInfo,
        Vector2Int rakelMappedInfoSize,
        Vector2Int rakelReservoirSize,
        ShaderRegion shaderRegion,
        ReduceFunction reduceFunction,
        ComputeBuffer resultTarget)
    {
        if (reduceFunction == ReduceFunction.Avg)
        {
            // count pixels under paint source
            ComputeBuffer activeCount = new ComputeBuffer(1, sizeof(int));
            int[] activeCountData = new int[1];
            activeCount.SetData(activeCountData);

            new ComputeShaderTask(
                "Reservoir/CountActive",
                shaderRegion,
                new List<CSAttribute>()
                {
                new CSComputeBuffer("RakelMappedInfo", rakelMappedInfo),
                new CSInt2("RakelMappedInfoSize", rakelMappedInfoSize),
                new CSInt2("RakelReservoirSize", rakelReservoirSize),

                new CSComputeBuffer("Workspace", Workspace),
                new CSInt3("WorkspaceSize", Size),

                new CSComputeBuffer("ActiveCount", activeCount),
                },
                false
            ).Run();

            // do add reduce and divide by value to get average
            ReduceWorkspace(
                shaderRegion,
                InternalReduceFunction.Add,
                false);

            // divide by count and do add reduce to get average
            new ComputeShaderTask(

                "Reservoir/DivideByValue",
                new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
                new List<CSAttribute>()
                {
                new CSComputeBuffer("Workspace", Workspace),
                new CSInt2("WorkspaceSize", Size2D),
                new CSInt2("DividendPosition", shaderRegion.Position),

                new CSComputeBuffer("Divisor", activeCount),
                },
                false
            ).Run();

            activeCount.Dispose();
        }
        else
        {
            ReduceWorkspace(
                shaderRegion,
                InternalReduceFunction.Max,
                false);
        }

        ExtractReducedValue(shaderRegion, resultTarget);
    }

    // NOTE: It is assumed that the data is copied to workspace already
    public void ReduceActiveWorkspaceMax(
        ShaderRegion shaderRegion,
        ComputeBuffer resultTarget)
    {
        // do add reduce and divide by value to get average
        ReduceWorkspace(
            shaderRegion,
            InternalReduceFunction.Max,
            false);

        ExtractReducedValue(shaderRegion, resultTarget);
    }

    // NOTE: It is assumed that the data is copied to workspace already
    public void ReduceWorkspaceMax(
        ShaderRegion shaderRegion,
        ComputeBuffer resultTarget)
    {
        ReduceWorkspace(
            shaderRegion,
            InternalReduceFunction.Max,
            false);

        ExtractReducedValue(shaderRegion, resultTarget);
    }

    public void ExtractReducedValue(
        ShaderRegion shaderRegion,
        ComputeBuffer resultTarget)
    {
        new ComputeShaderTask(
            "Reservoir/ExtractReducedValue",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            new List<CSAttribute>()
            {
                new CSComputeBuffer("ReducedValueSource", Workspace),
                new CSInt2("ReducedValueSourceSize", Size2D),
                new CSInt2("ReducedValueSourceIndex", shaderRegion.Position),

                new CSComputeBuffer("ReducedValueTarget", resultTarget),
            },
            false
        ).Run();
    }

    public void ReduceWorkspace(ShaderRegion reduceRegion, InternalReduceFunction reduceFunction, bool debugEnabled = false)
    {
        // shader is hardcoded to deal with 2x2 blocks (processing 4 values per thread)
        Vector2Int REDUCE_BLOCK_SIZE = new Vector2Int(2, 2);

        while (reduceRegion.Size.x > 1 || reduceRegion.Size.y > 1)
        {
            // reduceShaderRegion is the region the shader runs on to perform the reduce on the reduceRegion
            ShaderRegion reduceShaderRegion = new ShaderRegion(
                reduceRegion.Position,
                reduceRegion.Size,
                REDUCE_BLOCK_SIZE);

            new ComputeShaderTask(
                "Reservoir/ReduceWorkspace",
                reduceShaderRegion,
                new List<CSAttribute>
                {
                    new CSComputeBuffer("Workspace", Workspace),
                    new CSInt3("WorkspaceSize", Size),
                    new CSInt2("ReduceRegionSize", reduceRegion.Size),
                    new CSInt("ReduceFunction", (int) reduceFunction),
                },
                debugEnabled
            ).Run();

            // current reduceShaderRegion is new reduceRegion
            reduceRegion = reduceShaderRegion;
        }
    }

    public override void Dispose()
    {
        base.Dispose();

        PaintGridSnapshot.Dispose();
        PaintGridSnapshotImprintCopy.Dispose();
        PaintGridInfoSnapshot.Dispose();
        Workspace.Dispose();
    }
}
