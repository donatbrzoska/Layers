using UnityEngine;
using System.Collections.Generic;

public class Reservoir
{
    public int Resolution;
    public Vector2Int Size;
    public ComputeBuffer Buffer; // 3D array, z=1 is for duplication for correct interpolation
    private Paint[] BufferData;

    public float PixelSize { get { return 1 / (float) Resolution; } }

    private ShaderRegionFactory ShaderRegionFactory;
    private Queue<ComputeShaderTask> ComputeShaderTasks;

    public Reservoir(int resolution, int width, int height, ShaderRegionFactory shaderRegionFactory, Queue<ComputeShaderTask> computeShaderTasks)
    {
        Resolution = resolution;
        Size = new Vector2Int(width, height);

        Buffer = new ComputeBuffer(width * height * 2, Paint.SizeInBytes);
        // initialize buffer to empty values (Intel does this for you, nvidia doesn't)
        BufferData = new Paint[width * height * 2];
        Buffer.SetData(BufferData);

        ShaderRegionFactory = shaderRegionFactory;
        ComputeShaderTasks = computeShaderTasks;
    }

    public void Fill(Color_ color, int volume, ReservoirFiller filler)
    {
        filler.Fill(color, volume, BufferData, Size);
        Buffer.SetData(BufferData);
    }

    private void EnqueueOrRun(ComputeShaderTask cst)
    {
        if (ComputeShaderTasks != null)
        {
            ComputeShaderTasks.Enqueue(cst);
        }
        else
        {
            cst.Run();
        }
    }

    public void Duplicate(
        int discardVolumeThreshold,
        int smoothingKernelSize,
        bool debugEnabled = false)
    {
        ShaderRegion duplicateSR = ShaderRegionFactory.Create(
            new Vector2Int(0, Size.y - 1),
            new Vector2Int(Size.x - 1, Size.y - 1),
            new Vector2Int(0, 0),
            new Vector2Int(Size.x - 1, 0)
        );

        List<CSAttribute> attributes = new List<CSAttribute>()
        {
            new CSComputeBuffer("Reservoir", Buffer),
            new CSInt("DiscardVolumeThreshhold", discardVolumeThreshold),
            new CSInt("SmoothingKernelSize", smoothingKernelSize)
        };

        ComputeShaderTask cst = new ComputeShaderTask(
            "ReservoirDuplicationShader",
            duplicateSR,
            attributes,
            null,
            new List<ComputeBuffer>(),
            debugEnabled
        );

        EnqueueOrRun(cst);
    }

    public void Dispose()
    {
        Buffer.Dispose();
    }
}
