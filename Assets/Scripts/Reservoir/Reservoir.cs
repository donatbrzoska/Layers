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
    public ComputeBuffer PaintGridInfoWorkspace;

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
        PaintGridInfoWorkspace = new ComputeBuffer(Size.x * Size.y, ColumnInfo.SizeInBytes);
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

    public void CopySnapshotActiveInfoToWorkspace(
        ComputeBuffer paintSourceMappedInfo,
        Vector2Int paintSourceReservoirSize,
        ShaderRegion shaderRegion,
        bool debugEnabled = false)
    {
        DuplicateActiveInfo_(
            paintSourceMappedInfo,
            paintSourceReservoirSize,
            shaderRegion,
            PaintGridInfoSnapshot,
            PaintGridInfoWorkspace,
            debugEnabled);
    }
    
    public void DuplicateActiveInfo(
        ComputeBuffer paintSourceMappedInfo,
        Vector2Int paintSourceReservoirSize,
        ShaderRegion shaderRegion,
        bool debugEnabled = false)
    {
        DuplicateActiveInfo_(
            paintSourceMappedInfo,
            paintSourceReservoirSize,
            shaderRegion,
            PaintGrid.Info,
            PaintGridSampleSource.Info,
            debugEnabled);
    }

    private void DuplicateActiveInfo_(
        ComputeBuffer paintSourceMappedInfo,
        Vector2Int paintSourceReservoirSize,
        ShaderRegion shaderRegion,
        ComputeBuffer source,
        ComputeBuffer target,
        bool debugEnabled = false)
    {
        new ComputeShaderTask(
            "Reservoir/DuplicateActiveInfo",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("PaintSourceMappedInfo", paintSourceMappedInfo),
                new CSInt2("PaintSourceReservoirSize", paintSourceReservoirSize),

                new CSComputeBuffer("ReservoirInfo", source),
                new CSComputeBuffer("ReservoirInfoDuplicate", target),
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

    // NOTE: It is assumed that the info is written to workspace already
    public void ReducePaintGridInfoWorkspaceVolumeAvg(
        ComputeBuffer paintSourceMappedInfo,
        Vector2Int paintSourceReservoirSize,
        ShaderRegion paintTargetSR,
        ComputeBuffer resultTarget)
    {
        ReduceInfoVolumeAvg_(
            paintSourceMappedInfo,
            paintSourceReservoirSize,
            paintTargetSR,
            PaintGridInfoWorkspace,
            resultTarget);
    }
    
    // NOTE: It is assumed that the info is duplicated already
    public void ReduceInfoVolumeAvg(
        ComputeBuffer paintSourceMappedInfo,
        Vector2Int paintSourceReservoirSize,
        ShaderRegion paintTargetSR,
        ComputeBuffer resultTarget)
    {
        ReduceInfoVolumeAvg_(
            paintSourceMappedInfo,
            paintSourceReservoirSize,
            paintTargetSR,
            PaintGridSampleSource.Info,
            resultTarget);
    }

    private void ReduceInfoVolumeAvg_(
        ComputeBuffer paintSourceMappedInfo,
        Vector2Int paintSourceReservoirSize,
        ShaderRegion paintTargetSR,
        ComputeBuffer info,
        ComputeBuffer resultTarget)
    {
        // count pixels under paint source
        ComputeBuffer activeCount = new ComputeBuffer(1, sizeof(int));
        int[] activeCountData = new int[1];
        activeCount.SetData(activeCountData);

        new ComputeShaderTask(
            "Reservoir/CountActive",
            paintTargetSR,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("PaintSourceMappedInfo", paintSourceMappedInfo),
                new CSInt2("PaintSourceReservoirSize", paintSourceReservoirSize),

                new CSComputeBuffer("ReservoirInfo", info),
                new CSInt3("ReservoirSize", Size),

                new CSComputeBuffer("ActiveCount", activeCount),
            },
            false
        ).Run();

        // do add reduce and divide by value to get average
        ReduceInfoVolume(
            paintTargetSR,
            ReduceFunction.Add,
            info,
            false);

        // divide by count and do add reduce to get average
        new ComputeShaderTask(

            "Reservoir/DivideByValue",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            new List<CSAttribute>()
            {
                new CSComputeBuffer("ReservoirInfo", info),
                new CSInt2("ReservoirSize", new Vector2Int(Size.x, Size.y)),
                new CSInt2("DividendPosition", paintTargetSR.Position),

                new CSComputeBuffer("Divisor", activeCount),
            },
            false
        ).Run();

        activeCount.Dispose();

        // return result
        new ComputeShaderTask(
            "Reservoir/ExtractReducedVolume",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            new List<CSAttribute>()
            {
                new CSComputeBuffer("ReducedVolumeSource", info),
                new CSInt2("ReducedVolumeSourceSize", new Vector2Int(Size.x, Size.y)),
                new CSInt2("ReducedVolumeSourceIndex", paintTargetSR.Position),

                new CSComputeBuffer("ReducedVolumeTarget", resultTarget),
            },
            false
        ).Run();
    }

    // NOTE: It is assumed that the reservoir is duplicated already
    public void ReduceInfoDuplicateVolumeMax(
        ShaderRegion paintTargetSR,
        ComputeBuffer resultTarget)
    {
        ReduceInfoVolume(
            paintTargetSR,
            ReduceFunction.Max,
            PaintGridSampleSource.Info,
            false);

        // return result
        new ComputeShaderTask(
            "Reservoir/ExtractReducedVolume",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            new List<CSAttribute>()
            {
                new CSComputeBuffer("ReducedVolumeSource", PaintGridSampleSource.Info),
                new CSInt2("ReducedVolumeSourceSize", new Vector2Int(Size.x, Size.y)),
                new CSInt2("ReducedVolumeSourceIndex", paintTargetSR.Position),

                new CSComputeBuffer("ReducedVolumeTarget", resultTarget),
            },
            false
        ).Run();
    }


    public void ReduceInfoDuplicateVolume(ShaderRegion reduceRegion, ReduceFunction reduceFunction, bool debugEnabled = false)
    {
        ReduceInfoVolume(reduceRegion, reduceFunction, PaintGridSampleSource.Info, debugEnabled);
    }

    private void ReduceInfoVolume(ShaderRegion reduceRegion, ReduceFunction reduceFunction, ComputeBuffer info, bool debugEnabled = false)
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
                "Reservoir/ReduceInfoVolume",
                reduceShaderRegion,
                new List<CSAttribute>
                {
                    new CSComputeBuffer("ReservoirInfo", info),
                    new CSInt3("ReservoirSize", Size),
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
        PaintGridInfoWorkspace.Dispose();
    }
}
