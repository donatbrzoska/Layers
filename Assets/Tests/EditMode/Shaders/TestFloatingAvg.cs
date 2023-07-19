using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestApplyFloatingAvg
{
    ComputeBuffer Volume;
    float[] VolumeData;
    ComputeBuffer AvgRingbuffer;
    float[] AvgRingbufferData;
    bool StrokeBegin;

    private void Execute()
    {
        Volume = new ComputeBuffer(1, sizeof(float));
        Volume.SetData(VolumeData);
        AvgRingbuffer = new ComputeBuffer(AvgRingbufferData.Length, sizeof(float));
        AvgRingbuffer.SetData(AvgRingbufferData);

        new ComputeShaderTask(
            "Tests/TestFloatingAvg",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            new List<CSAttribute>()
            {
                    new CSComputeBuffer("ValueSourceSink", Volume),

                    new CSComputeBuffer("AvgRingbuffer", AvgRingbuffer),
                    new CSInt("StrokeBegin", StrokeBegin ? 1 : 0)
            },
            true
        ).Run();

        Volume.GetData(VolumeData);
        AvgRingbuffer.GetData(AvgRingbufferData);
    }

    [TearDown]
    public void Teardown()
    {
        Volume.Dispose();
        AvgRingbuffer.Dispose();
    }

    [Test]
    public void init()
    {
        int RINGBUF_SIZE = 4;

        // Arrange
        VolumeData = new float[] { 2 };
        AvgRingbufferData = new float[] { 0, 0, RINGBUF_SIZE, 0, 0, 0, 0 };
        StrokeBegin = true;

        // Act
        Execute();

        // Assert
        Assert.AreEqual(
            new float[] { 2 },
            VolumeData);

        Assert.AreEqual(
            new float[] { 2, 0, RINGBUF_SIZE, 2, 2, 2, 2 },
            AvgRingbufferData);
    }

    [Test]
    public void new_value()
    {
        int RINGBUF_SIZE = 4;

        // Arrange
        VolumeData = new float[] { 4 };
        AvgRingbufferData = new float[] { 2, 0, RINGBUF_SIZE, 2, 2, 2, 2 };
        StrokeBegin = false;

        // Act
        Execute();

        // Assert
        Assert.AreEqual(
            new float[] { 2.5f },
            VolumeData);

        Assert.AreEqual(
            new float[] { 2.5f, 1, RINGBUF_SIZE, 4, 2, 2, 2 },
            AvgRingbufferData);
    }

    [Test]
    public void new_value_wrap_around_pointer()
    {
        int RINGBUF_SIZE = 4;

        // Arrange
        VolumeData = new float[] { 4 };
        AvgRingbufferData = new float[] { 2, 3, RINGBUF_SIZE, 2, 2, 2, 2 };
        StrokeBegin = false;

        // Act
        Execute();

        // Assert
        Assert.AreEqual(
            new float[] { 2.5f },
            VolumeData);

        Assert.AreEqual(
            new float[] { 2.5f, 0, RINGBUF_SIZE, 2, 2, 2, 4 },
            AvgRingbufferData);
    }
}