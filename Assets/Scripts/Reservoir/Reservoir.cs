using UnityEngine;
using System.Collections.Generic;

public enum ReduceFunction
{
    Max,
    Add,
}

public class Reservoir
{
    public int Resolution;
    public Vector3Int Size;

    public PaintGrid PaintGrid;
    public PaintGrid PaintGridSampleSource;

    public ComputeBuffer PaintGridInfoSnapshot;
    public ComputeBuffer Workspace;

    public float PixelSize { get { return 1 / (float) Resolution; } }

    public Reservoir(int resolution, int width, int height, int layers, float cellVolume, int diffuseDepth, float diffuseRatio)
    {
        Resolution = resolution;
        Size = new Vector3Int(width, height, layers);

        PaintGrid = new PaintGrid(Size, cellVolume, diffuseDepth, diffuseRatio);
        PaintGridSampleSource = new PaintGrid(Size, cellVolume, diffuseDepth, diffuseRatio);

        // only used by canvas
        // TODO provide possibility to deactivate this
        PaintGridInfoSnapshot = new ComputeBuffer(Size.x * Size.y, ColumnInfo.SizeInBytes);
        Workspace = new ComputeBuffer(Size.x * Size.y, sizeof(float));
    }

    public void Fill(ReservoirFiller filler)
    {
        PaintGrid.Fill(filler);
    }

    public ShaderRegion GetFullShaderRegion()
    {
        return new ShaderRegion(
            new Vector2Int(0, Size.y - 1),
            new Vector2Int(Size.x - 1, Size.y - 1),
            new Vector2Int(0, 0),
            new Vector2Int(Size.x - 1, 0)
        );
    }

    public void Duplicate(bool debugEnabled = false)
    {
        Duplicate(GetFullShaderRegion(), debugEnabled);
    }

    public void Duplicate(ShaderRegion sr, bool debugEnabled = false)
    {
        new ComputeShaderTask(
            "Reservoir/ReservoirDuplication",
            sr,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("ReservoirInfo", PaintGrid.Info),
                new CSComputeBuffer("ReservoirContent", PaintGrid.Content),
                new CSComputeBuffer("ReservoirInfoDuplicate", PaintGridSampleSource.Info),
                new CSComputeBuffer("ReservoirContentDuplicate", PaintGridSampleSource.Content),
                new CSInt3("ReservoirSize", Size),
            },
            debugEnabled
        ).Run();
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
        ComputeBuffer paintSourceMappedInfo,
        Vector2Int paintSourceReservoirSize,
        ShaderRegion shaderRegion,
        bool debugEnabled = false)
    {
        new ComputeShaderTask(
            "Reservoir/CopyActiveInfoVolumesToWorkspace",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("PaintSourceMappedInfo", paintSourceMappedInfo),
                new CSInt2("PaintSourceReservoirSize", paintSourceReservoirSize),

                new CSComputeBuffer("ReservoirInfo", PaintGridInfoSnapshot),
                new CSComputeBuffer("Workspace", Workspace),
                new CSInt3("ReservoirSize", Size)
            },
            debugEnabled
        ).Run();
    }

    public void PrintVolumes(int z)
    {
        PaintGrid.Content.GetData(PaintGrid.ContentData);
        LogUtil.LogVolumes(PaintGrid.ContentData, GetFullShaderRegion().Size.y, GetFullShaderRegion().Size.x, z, "z=" + z);

        //int sum = 0;
        //for (int i = 0; i < BufferData.GetLength(0) / 2; i++)
        //{
        //    sum += (int)BufferData[i].Volume;
        //}
        //Debug.Log("Sum is " + sum);
    }

    // NOTE: It is assumed that the data is copied to workspace already
    public void ReduceActiveWorkspaceAvg(
        ComputeBuffer paintSourceMappedInfo,
        Vector2Int paintSourceReservoirSize,
        ShaderRegion shaderRegion,
        ComputeBuffer resultTarget)
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
                new CSComputeBuffer("PaintSourceMappedInfo", paintSourceMappedInfo),
                new CSInt2("PaintSourceReservoirSize", paintSourceReservoirSize),

                new CSComputeBuffer("Workspace", Workspace),
                new CSInt3("WorkspaceSize", Size),

                new CSComputeBuffer("ActiveCount", activeCount),
            },
            false
        ).Run();

        // do add reduce and divide by value to get average
        ReduceWorkspace(
            shaderRegion,
            ReduceFunction.Add,
            false);

        // divide by count and do add reduce to get average
        new ComputeShaderTask(

            "Reservoir/DivideByValue",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            new List<CSAttribute>()
            {
                new CSComputeBuffer("Workspace", Workspace),
                new CSInt2("WorkspaceSize", new Vector2Int(Size.x, Size.y)),
                new CSInt2("DividendPosition", shaderRegion.Position),

                new CSComputeBuffer("Divisor", activeCount),
            },
            false
        ).Run();

        activeCount.Dispose();

        ExtractReducedValue(shaderRegion, resultTarget);
    }

    // NOTE: It is assumed that the data is copied to workspace already
    public void ReduceWorkspaceMax(
        ShaderRegion shaderRegion,
        ComputeBuffer resultTarget)
    {
        ReduceWorkspace(
            shaderRegion,
            ReduceFunction.Max,
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
                new CSInt2("ReducedValueSourceSize", new Vector2Int(Size.x, Size.y)),
                new CSInt2("ReducedValueSourceIndex", shaderRegion.Position),

                new CSComputeBuffer("ReducedValueTarget", resultTarget),
            },
            false
        ).Run();
    }

    public void ReduceWorkspace(ShaderRegion reduceRegion, ReduceFunction reduceFunction, bool debugEnabled = false)
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

    public void Dispose()
    {
        PaintGrid.Dispose();
        PaintGridSampleSource.Dispose();

        PaintGridInfoSnapshot.Dispose();
        Workspace.Dispose();
    }
}
