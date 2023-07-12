using UnityEngine;
using System.Collections.Generic;
using System;

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
    public PaintGrid PaintGridDuplicate;

    public float PixelSize { get { return 1 / (float) Resolution; } }

    private ComputeShaderEngine ComputeShaderEngine;

    public Reservoir(int resolution, int width, int height, int layers, float cellVolume, int diffuseDepth, float diffuseRatio, ComputeShaderEngine computeShaderEngine)
    {
        Resolution = resolution;
        Size = new Vector3Int(width, height, layers);

        PaintGrid = new PaintGrid(Size, cellVolume, diffuseDepth, diffuseRatio);
        PaintGridDuplicate = new PaintGrid(Size, cellVolume, diffuseDepth, diffuseRatio);

        ComputeShaderEngine = computeShaderEngine;
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
        ComputeShaderEngine.EnqueueOrRun(new ComputeShaderTask(
            "Reservoir/ReservoirDuplication",
            sr,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("ReservoirInfo", PaintGrid.Info),
                new CSComputeBuffer("ReservoirContent", PaintGrid.Content),
                new CSComputeBuffer("ReservoirInfoDuplicate", PaintGridDuplicate.Info),
                new CSComputeBuffer("ReservoirContentDuplicate", PaintGridDuplicate.Content),
                new CSInt3("ReservoirSize", Size),
            },
            null,
            debugEnabled
        ));
    }

    private void DuplicateActive(
        ComputeBuffer paintSourceMappedInfo,
        Vector2Int paintSourceReservoirSize,
        ShaderRegion shaderRegion,
        bool debugEnabled = false)
    {
        ComputeShaderEngine.EnqueueOrRun(new ComputeShaderTask(
            "Reservoir/ActiveReservoirDuplication",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("PaintSourceMappedInfo", paintSourceMappedInfo),
                new CSInt2("PaintSourceReservoirSize", paintSourceReservoirSize),

                new CSComputeBuffer("ReservoirInfo", PaintGrid.Info),
                new CSComputeBuffer("ReservoirInfoDuplicate", PaintGridDuplicate.Info),
                new CSInt3("ReservoirSize", Size)
            },
            null,
            debugEnabled
        ));
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

    // NOTE: It is assumed that the reservoir is duplicated already
    public void ReduceVolumeAvg(
        ComputeBuffer paintSourceMappedInfo,
        Vector2Int paintSourceReservoirSize,
        ShaderRegion paintTargetSR,
        ComputeBuffer resultTarget)
    {
        // count pixels under paint source
        ComputeBuffer activeCount = new ComputeBuffer(1, sizeof(int));
        int[] activeCountData = new int[1];
        activeCount.SetData(activeCountData);

        ComputeShaderEngine.EnqueueOrRun(new ComputeShaderTask(
            "Reservoir/CountActive",
            paintTargetSR,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("PaintSourceMappedInfo", paintSourceMappedInfo),
                new CSInt2("PaintSourceReservoirSize", paintSourceReservoirSize),

                new CSComputeBuffer("ActiveCount", activeCount),
            },
            null,
            false
        ));

        // divide by count and do add reduce to get average
        ComputeShaderEngine.EnqueueOrRun(new ComputeShaderTask(
            "RakelState/DivideByValue",
            paintTargetSR,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("Value", activeCount),

                new CSComputeBuffer("ReservoirInfoDuplicate", PaintGridDuplicate.Info),
                new CSInt2("ReservoirSize", new Vector2Int(Size.x, Size.y))
            },
            new List<IDisposable> { activeCount },
            false
        ));

        ReduceVolume(
            paintTargetSR,
            ReduceFunction.Add,
            false);

        // return result
        ComputeShaderEngine.EnqueueOrRun(new ComputeShaderTask(
            "Reservoir/ExtractReducedVolume",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            new List<CSAttribute>()
            {
                new CSComputeBuffer("ReducedVolumeSource", PaintGridDuplicate.Info),
                new CSInt2("ReducedVolumeSourceSize", new Vector2Int(Size.x, Size.y)),
                new CSInt2("ReducedVolumeSourceIndex", paintTargetSR.Position),

                new CSComputeBuffer("ReducedVolumeTarget", resultTarget),
            },
            null,
            false
        ));
    }

    public void ReduceVolume(ShaderRegion reduceRegion, ReduceFunction reduceFunction, bool debugEnabled = false)
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

            ComputeShaderEngine.EnqueueOrRun(new ComputeShaderTask(
                "Reservoir/ReduceVolume",
                reduceShaderRegion,
                new List<CSAttribute>
                {
                    new CSComputeBuffer("ReservoirInfoDuplicate", PaintGridDuplicate.Info),
                    new CSInt3("ReservoirSize", Size),
                    new CSInt2("ReduceRegionSize", reduceRegion.Size),
                    new CSInt("ReduceFunction", (int) reduceFunction),
                },
                null,
                debugEnabled
            ));

            // current reduceShaderRegion is new reduceRegion
            reduceRegion = reduceShaderRegion;
        }
    }

    public void Dispose()
    {
        PaintGrid.Dispose();
        PaintGridDuplicate.Dispose();
    }
}
