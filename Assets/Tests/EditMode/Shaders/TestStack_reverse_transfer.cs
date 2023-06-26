using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestStack_reverse_transfer
{
    private const int KERNEL_ID_reverse_tranfer = 3;

    List<CSAttribute> Attributes;

    ComputeBuffer SourceStack2DInfo;
    StackInfo[] SourceStack2DInfoData;
    ComputeBuffer SourceStack2DContent;
    Paint[] SourceStack2DContentData;
    Vector2Int SourceStack2DSize;

    ComputeBuffer TargetStack2DInfo;
    StackInfo[] TargetStack2DInfoData;
    ComputeBuffer TargetStack2DContent;
    Paint[] TargetStack2DContentData;
    Vector2Int TargetStack2DSize;
    Vector2Int TargetStack2DPosition;


    [SetUp]
    public void Setup()
    {
        Attributes = new List<CSAttribute>();

        new FileLogger_().OnEnable();
    }

    [TearDown]
    public void Teardown()
    {
        SourceStack2DInfo.Dispose();
        SourceStack2DContent.Dispose();
        TargetStack2DInfo.Dispose();
        TargetStack2DContent.Dispose();

        new FileLogger_().OnDisable();
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        SourceStack2DInfo = new ComputeBuffer(SourceStack2DSize.x * SourceStack2DSize.y, StackInfo.SizeInBytes);
        SourceStack2DInfo.SetData(SourceStack2DInfoData);
        SourceStack2DContent = new ComputeBuffer(SourceStack2DSize.x * SourceStack2DSize.y * SourceStack2DInfoData[0].MaxSize, Paint.SizeInBytes);
        SourceStack2DContent.SetData(SourceStack2DContentData);

        TargetStack2DInfo = new ComputeBuffer(TargetStack2DSize.x * TargetStack2DSize.y, StackInfo.SizeInBytes);
        TargetStack2DInfo.SetData(TargetStack2DInfoData);
        TargetStack2DContent = new ComputeBuffer(TargetStack2DSize.x * TargetStack2DSize.y * TargetStack2DInfoData[0].MaxSize, Paint.SizeInBytes);
        TargetStack2DContent.SetData(TargetStack2DContentData);

        Attributes.Add(new CSComputeBuffer("Source2DInfo", SourceStack2DInfo));
        Attributes.Add(new CSComputeBuffer("Source2DContent", SourceStack2DContent));
        Attributes.Add(new CSInt2("Source2DSize", SourceStack2DSize));

        Attributes.Add(new CSComputeBuffer("Target2DInfo", TargetStack2DInfo));
        Attributes.Add(new CSComputeBuffer("Target2DContent", TargetStack2DContent));
        Attributes.Add(new CSInt2("Target2DSize", TargetStack2DSize));
        Attributes.Add(new CSInt2("Target2DPosition", TargetStack2DPosition));

        ComputeShaderTask cst = new ComputeShaderTask(
            "Tests/TestStack",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            Attributes,
            true,
            kernelID);

        cst.Run();

        TargetStack2DContent.GetData(TargetStack2DContentData);
        TargetStack2DInfo.GetData(TargetStack2DInfoData);

        return cst;
    }

    Paint P(float v)
    {
        return new Paint(new Color(v, v, v, 1), v);
    }

    Paint P(float color, float volume)
    {
        return new Paint(new Color(color, color, color, 1), volume);
    }

    [Test]
    public void empty_source_stack_does_nothing()
    {
        // Arrange
        SourceStack2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 0, MaxSize = 1, WriteIndex = 0, Volume = 0 }
        };
        SourceStack2DContentData = new Paint[]
        {
            P(0),
        };
        SourceStack2DSize = new Vector2Int(1, 1);

        TargetStack2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 0, MaxSize = 1, WriteIndex = 0, Volume = 0 }
        };
        TargetStack2DContentData = new Paint[]
        {
            P(-1),
        };
        TargetStack2DSize = new Vector2Int(1, 1);
        TargetStack2DPosition = Vector2Int.zero;


        // Act
        Execute(KERNEL_ID_reverse_tranfer);


        // Assert
        Assert.AreEqual(
            new StackInfo[]
            {
                new StackInfo { Size = 0, MaxSize = 1, WriteIndex = 0, Volume = 0 }
            },
            TargetStack2DInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(-1)
            },
            TargetStack2DContentData);
    }

    [Test]
    public void correct_position()
    {
        // Arrange
        SourceStack2DInfoData = new StackInfo[]
        {
            // Notice that write index is 1 because we simulate a preceding raw_push
            new StackInfo { Size = 1, MaxSize = 1, WriteIndex = 1, Volume = 0.6f } 
        };
        SourceStack2DContentData = new Paint[]
        {
            P(0.6f),
        };
        SourceStack2DSize = new Vector2Int(1, 1);

        TargetStack2DInfoData = new StackInfo[]
        {
                new StackInfo { Size = 0, MaxSize = 1, WriteIndex = 0, Volume = 0 }, new StackInfo { Size = 0, MaxSize = 1, WriteIndex = 0, Volume = 0 },
                new StackInfo { Size = 0, MaxSize = 1, WriteIndex = 0, Volume = 0 }, new StackInfo { Size = 0, MaxSize = 1, WriteIndex = 0, Volume = 0 },
                new StackInfo { Size = 0, MaxSize = 1, WriteIndex = 0, Volume = 0 }, new StackInfo { Size = 0, MaxSize = 1, WriteIndex = 0, Volume = 0 },
        };
        TargetStack2DContentData = new Paint[]
        {
            P(-1), P(-1),
            P(-1), P(-1),
            P(-1), P(-1)
        };
        TargetStack2DSize = new Vector2Int(2, 3);
        TargetStack2DPosition = new Vector2Int(1, 2);


        // Act
        Execute(KERNEL_ID_reverse_tranfer);


        // Assert
        Assert.AreEqual(
            new StackInfo[]
            {
                new StackInfo { Size = 0, MaxSize = 1, WriteIndex = 0, Volume = 0 }, new StackInfo { Size = 0, MaxSize = 1, WriteIndex = 0, Volume = 0 },
                new StackInfo { Size = 0, MaxSize = 1, WriteIndex = 0, Volume = 0 }, new StackInfo { Size = 0, MaxSize = 1, WriteIndex = 0, Volume = 0 },
                new StackInfo { Size = 0, MaxSize = 1, WriteIndex = 0, Volume = 0 }, new StackInfo { Size = 1, MaxSize = 1, WriteIndex = 0, Volume = 0.6f }
            },
            TargetStack2DInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(-1), P(-1),
                P(-1), P(-1),
                P(-1), P(0.6f)
            },
            TargetStack2DContentData);
    }

    [Test]
    public void correct_mix_in()
    {
        // Arrange
        SourceStack2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 1, MaxSize = 1, WriteIndex = 1, Volume = 0.8f }
        };
        SourceStack2DContentData = new Paint[]
        {
            P(0, 0.6f),
        };
        SourceStack2DSize = new Vector2Int(1, 1);

        TargetStack2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 2, MaxSize = 2, WriteIndex = 1, Volume = 1.2f }
        };
        TargetStack2DContentData = new Paint[]
        {
            P(1),

            P(1, 0.2f),
        };
        TargetStack2DSize = new Vector2Int(1, 1);
        TargetStack2DPosition = Vector2Int.zero;


        // Act
        Execute(KERNEL_ID_reverse_tranfer);


        // Assert
        Assert.AreEqual(
            new StackInfo[]
            {
                new StackInfo { Size = 2, MaxSize = 2, WriteIndex = 1, Volume = 1.8f }
            },
            TargetStack2DInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(1),

                P(0.25f, 0.8f),
            },
            TargetStack2DContentData);
    }

    [Test]
    public void multiple_elements()
    {
        // Arrange
        SourceStack2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 2, MaxSize = 2, WriteIndex = 2, Volume = 1.6f }
        };
        SourceStack2DContentData = new Paint[]
        {
            P(1, 0.6f),

            P(0, 1),
        };
        SourceStack2DSize = new Vector2Int(1, 1);

        TargetStack2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 0, MaxSize = 2, WriteIndex = 0, Volume = 0 }
        };
        TargetStack2DContentData = new Paint[]
        {
            P(-1),

            P(-1),
        };
        TargetStack2DSize = new Vector2Int(1, 1);
        TargetStack2DPosition = Vector2Int.zero;


        // Act
        Execute(KERNEL_ID_reverse_tranfer);


        // Assert
        Assert.AreEqual(
            new StackInfo[]
            {
                new StackInfo { Size = 2, MaxSize = 2, WriteIndex = 1, Volume = 1.6f }
            },
            TargetStack2DInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0.6f, 1),

                P(0, 0.6f),
            },
            TargetStack2DContentData);
    }
}