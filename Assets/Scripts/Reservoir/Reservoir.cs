using UnityEngine;
using System.Collections.Generic;

public enum ReduceFunction
{
    Max,
    Add,
}

public struct StackInfo
{
    public static int SizeInBytes = 3 * sizeof(int) + sizeof(float);

    public int Size;
    public int MaxSize;
    public int WriteIndex;
    public float Volume;

    public StackInfo(int size, int maxSize, int writeIndex, float volume)
    {
        Size = size;
        MaxSize = maxSize;
        WriteIndex = writeIndex;
        Volume = volume;
    }

    public override bool Equals(object other_)
    {
        StackInfo other = (StackInfo)other_;
        return Size == other.Size && MaxSize == other.MaxSize && WriteIndex == other.WriteIndex && Mathf.Abs(other.Volume - Volume) < 0.0001f;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString() + string.Format("(Size={0}, MaxSize={1}, WriteIndex={2}, Volume={3})", Size, MaxSize, WriteIndex, Volume);
    }

    public static bool operator ==(StackInfo a, StackInfo b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(StackInfo a, StackInfo b)
    {
        return !a.Equals(b);
    }
}

public class Reservoir
{
    public int Resolution;
    public Vector2Int Size;
    public ComputeBuffer Buffer;
    public ComputeBuffer BufferDuplicate;
    private Paint[] BufferData;

    public float PixelSize { get { return 1 / (float) Resolution; } }

    public Reservoir(int resolution, int width, int height)
    {
        Resolution = resolution;
        Size = new Vector2Int(width, height);

        Buffer = new ComputeBuffer(width * height, Paint.SizeInBytes);
        BufferDuplicate = new ComputeBuffer(width * height, Paint.SizeInBytes);
        BufferData = new Paint[width * height];
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
                new CSComputeBuffer("Reservoir", Buffer),
                new CSComputeBuffer("ReservoirDuplicate", BufferDuplicate)
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

                new CSComputeBuffer("Reservoir", Buffer),
                new CSComputeBuffer("ReservoirDuplicate", BufferDuplicate)
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
                    new CSComputeBuffer("ReservoirDuplicate", BufferDuplicate),
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
        BufferDuplicate.GetData(BufferData);
    }

    // Only used for testing purposes
    public Paint GetFromDuplicate(int x, int y)
    {
        return BufferData[IndexUtil.XY(x, y, Size.x)];
    }

    public void Dispose()
    {
        Buffer.Dispose();
        BufferDuplicate.Dispose();
    }
}
