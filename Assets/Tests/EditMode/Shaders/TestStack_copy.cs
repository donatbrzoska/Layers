using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestStack_copy
{
    private const int KERNEL_ID_copy = 0;

    List<CSAttribute> Attributes;

    [SetUp]
    public void Setup()
    {
        Attributes = new List<CSAttribute>();

        new FileLogger_().OnEnable();
    }

    [TearDown]
    public void Teardown()
    {
        new FileLogger_().OnDisable();
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        ComputeShaderTask cst = new ComputeShaderTask(
            "Tests/TestStack",
            new ShaderRegion(Vector2Int.zero, new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 1)),
            Attributes,
            true,
            kernelID);

        cst.Run();

        return cst;
    }

    Paint P(float v)
    {
        return new Paint(new Color(v, v, v, v), v);
    }

    [Test]
    public void copy()
    {
        Vector2Int ARRAY_SIZE = new Vector2Int(2, 2);
        int STACK_SIZE = 2;

        // Arrange
        ComputeBuffer source2DInfo = new ComputeBuffer(ARRAY_SIZE.x * ARRAY_SIZE.y, StackInfo.SizeInBytes);
        StackInfo[] source2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 0, WriteIndex = 0, Volume = 0 }, new StackInfo { Size = 1, WriteIndex = 0, Volume = 0.2f },
            new StackInfo { Size = 2, WriteIndex = 1, Volume = 1.4f }, new StackInfo { Size = 0, WriteIndex = 0, Volume = 0 },
        };
        source2DInfo.SetData(source2DInfoData);
        ComputeBuffer source2DContent = new ComputeBuffer(ARRAY_SIZE.x * ARRAY_SIZE.y * STACK_SIZE, Paint.SizeInBytes);
        Paint[] source2DContentData = new Paint[] // negative values should not be copied because out of range (see sourceInfoData)
        {
              P(-10),  P(0.2f),
              P(1f), P(-40),

              P(-20),  P(-30),
              P(0.4f), P(-50),
        };
        source2DContent.SetData(source2DContentData);

        ComputeBuffer target2DInfo = new ComputeBuffer(ARRAY_SIZE.x * ARRAY_SIZE.y, StackInfo.SizeInBytes);
        StackInfo[] target2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 0, WriteIndex = 0, Volume = 0 }, new StackInfo { Size = 0, WriteIndex = 0, Volume = 0 },
            new StackInfo { Size = 0, WriteIndex = 0, Volume = 0 }, new StackInfo { Size = 0, WriteIndex = 0, Volume = 0 },
        };
        target2DInfo.SetData(target2DInfoData);
        ComputeBuffer target2DContent = new ComputeBuffer(ARRAY_SIZE.x * ARRAY_SIZE.y * STACK_SIZE, Paint.SizeInBytes);
        Paint[] target2DContentData = new Paint[]
        {
            P(-1), P(-2),
            P(-3), P(-4),

            P(-5), P(-6),
            P(-7), P(-8),
        };
        target2DContent.SetData(target2DContentData);

        Attributes.Add(new CSComputeBuffer("Source2DInfo", source2DInfo));
        Attributes.Add(new CSComputeBuffer("Source2DContent", source2DContent));
        Attributes.Add(new CSComputeBuffer("Target2DInfo", target2DInfo));
        Attributes.Add(new CSComputeBuffer("Target2DContent", target2DContent));


        // Act
        Execute(KERNEL_ID_copy);


        // Assert
        target2DInfo.GetData(target2DInfoData);
        Assert.AreEqual(
            source2DInfoData,
            target2DInfoData);

        target2DContent.GetData(target2DContentData);
        Assert.AreEqual(
            new Paint[]
            {
                P(-1),   P(0.2f),
                P(1),    P(-4),

                P(-5),   P(-6),
                P(0.4f), P(-8),
            },
            target2DContentData);


        // Cleanup
        source2DInfo.Dispose();
        source2DContent.Dispose();
        target2DInfo.Dispose();
        target2DContent.Dispose();
    }
}