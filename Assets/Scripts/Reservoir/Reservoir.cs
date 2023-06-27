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
    public PaintGrid PaintGridDuplicate;

    public float PixelSize { get { return 1 / (float) Resolution; } }

    public Reservoir(int resolution, int width, int height, int layers)
    {
        Resolution = resolution;
        Size = new Vector3Int(width, height, layers);

        PaintGrid = new PaintGrid(Size);
        PaintGridDuplicate = new PaintGrid(Size);
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
        new ComputeShaderTask(
            "Reservoir/ReservoirDuplication",
            GetFullShaderRegion(),
            new List<CSAttribute>()
            {
                new CSComputeBuffer("ReservoirInfo", PaintGrid.Info),
                new CSComputeBuffer("ReservoirContent", PaintGrid.Content),
                new CSComputeBuffer("ReservoirInfoDuplicate", PaintGridDuplicate.Info),
                new CSComputeBuffer("ReservoirContentDuplicate", PaintGridDuplicate.Content),
                new CSInt3("ReservoirSize", Size),
            },
            debugEnabled
        ).Run();
    }

    public void DuplicateActive(
        ComputeBuffer paintSourceMappedInfo,
        Vector2Int paintSourceReservoirSize,
        ShaderRegion shaderRegion,
        bool debugEnabled = false)
    {
        new ComputeShaderTask(
            "Reservoir/ActiveReservoirDuplication",
            shaderRegion,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("PaintSourceMappedInfo", paintSourceMappedInfo),
                new CSInt2("PaintSourceReservoirSize", paintSourceReservoirSize),

                new CSComputeBuffer("ReservoirInfo", PaintGrid.Info),
                new CSComputeBuffer("ReservoirInfoDuplicate", PaintGridDuplicate.Info)
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

            new ComputeShaderTask(
                "Reservoir/ReduceVolume",
                reduceShaderRegion,
                new List<CSAttribute>()
                {
                    new CSComputeBuffer("ReservoirInfoDuplicate", PaintGridDuplicate.Info),
                    new CSInt3("ReservoirSize", Size),
                    new CSInt2("ReduceRegionSize", reduceRegion.Size),
                    new CSInt("ReduceFunction", (int) reduceFunction)
                },
                debugEnabled
            ).Run();

            // current reduceShaderRegion is new reduceRegion
            reduceRegion = reduceShaderRegion;
        }
    }

    // Only used for testing purposes
    public void Readback()
    {
        PaintGridDuplicate.Info.GetData(PaintGridDuplicate.InfoData);
    }

    // Only used for testing purposes
    public ColumnInfo GetFromDuplicate(int x, int y)
    {
        return PaintGridDuplicate.InfoData[IndexUtil.XY(x, y, Size.x)];
    }

    public void Dispose()
    {
        PaintGrid.Dispose();
        PaintGridDuplicate.Dispose();
    }
}
