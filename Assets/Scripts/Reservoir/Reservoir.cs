using UnityEngine;
using System.Collections.Generic;

public class Reservoir
{
    public int Resolution;
    public Vector3Int Size;
    public Vector2Int Size2D
    {
        private set { }

        get
        {
            return new Vector2Int(Size.x, Size.y);
        }
    }

    public PaintGrid PaintGrid;
    public PaintGrid PaintGridImprintCopy; // only read from for sampling

    public PaintGrid PaintGridInputBuffer;

    public float PixelSize { get { return 1 / (float) Resolution; } }

    public Reservoir(int resolution, int width, int height, int layers, float cellVolume)
    {
        Resolution = resolution;
        Size = new Vector3Int(width, height, layers);

        PaintGrid = new PaintGrid(Size, cellVolume);
        PaintGridImprintCopy = new PaintGrid(Size, cellVolume);

        // HACK InputBuffer is actually treated as a raw stack with no specified mixing parameters
        int UNUSED = 0;
        PaintGridInputBuffer = new PaintGrid(Size, UNUSED);
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

    public void DoImprintCopy(bool debugEnabled = false)
    {
        DoImprintCopy(GetFullShaderRegion(), debugEnabled);
    }

    public void DoImprintCopy(ShaderRegion sr, bool debugEnabled = false)
    {
        Duplicate(PaintGrid, PaintGridImprintCopy, sr, debugEnabled);
    }

    public void Duplicate(PaintGrid source, PaintGrid target, ShaderRegion sr, bool debugEnabled = false)
    {
        new ComputeShaderTask(
            "Reservoir/ReservoirDuplication",
            sr,
            new List<CSAttribute>()
            {
                new CSComputeBuffer("ReservoirInfo", source.Info),
                new CSComputeBuffer("ReservoirContent", source.Content),
                new CSComputeBuffer("ReservoirInfoDuplicate", target.Info),
                new CSComputeBuffer("ReservoirContentDuplicate", target.Content),
                new CSInt3("ReservoirSize", Size),
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

    public virtual void Dispose()
    {
        PaintGrid.Dispose();
        PaintGridImprintCopy.Dispose();

        PaintGridInputBuffer.Dispose();
    }
}
