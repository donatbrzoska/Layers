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
    public Vector2Int Size;
    public ComputeBuffer Buffer; // 3D array, z=1 is for duplication for correct interpolation
    private Paint[] BufferData;

    public float PixelSize { get { return 1 / (float) Resolution; } }

    public Reservoir(int resolution, int width, int height)
    {
        Resolution = resolution;
        Size = new Vector2Int(width, height);

        Buffer = new ComputeBuffer(width * height * 2, Paint.SizeInBytes);
        BufferData = new Paint[width * height * 2];
        Buffer.SetData(BufferData);
    }

    public void Fill(ReservoirFiller filler)
    {
        filler.Fill(BufferData, Size);
        Buffer.SetData(BufferData);
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
                new CSComputeBuffer("Reservoir", Buffer)
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

                new CSComputeBuffer("Reservoir", Buffer)
            },
            debugEnabled
        ).Run();
    }

    public void PrintVolumes(int z)
    {
        Buffer.GetData(BufferData);
        LogUtil.LogVolumes(BufferData, GetFullShaderRegion().Size.y, GetFullShaderRegion().Size.x, z, "z=" + z);

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
                    new CSComputeBuffer("Reservoir", Buffer),
                    new CSInt2("ReservoirSize", Size),
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
        Buffer.GetData(BufferData);
    }

    // Only used for testing purposes
    public Paint Get(int x, int y, int z)
    {
        return BufferData[IndexUtil.XYZ(x, y, z, Size.x, Size.y)];
    }

    public void Dispose()
    {
        Buffer.Dispose();
    }
}
