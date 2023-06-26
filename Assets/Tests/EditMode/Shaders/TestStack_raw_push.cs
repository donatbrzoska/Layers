using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestStack_raw_push
{
    private const int KERNEL_ID_raw_push = 2;

    List<CSAttribute> Attributes;

    ComputeBuffer Stack2DInfo;
    StackInfo[] Stack2DInfoData;
    ComputeBuffer Stack2DContent;
    Paint[] Stack2DContentData;
    ComputeBuffer NewElement;
    Paint[] NewElementData;

    [SetUp]
    public void Setup()
    {
        Attributes = new List<CSAttribute>();

        new FileLogger_().OnEnable();
    }

    [TearDown]
    public void Teardown()
    {
        Stack2DInfo.Dispose();
        Stack2DContent.Dispose();
        NewElement.Dispose();

        new FileLogger_().OnDisable();
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        Vector2Int stack2DSize = new Vector2Int(1, 1);
        Vector2Int newElementPosition = Vector2Int.zero;

        Stack2DInfo = new ComputeBuffer(stack2DSize.x * stack2DSize.y, StackInfo.SizeInBytes);
        Stack2DInfo.SetData(Stack2DInfoData);

        Stack2DContent = new ComputeBuffer(stack2DSize.x * stack2DSize.y * Stack2DInfoData[0].MaxSize, Paint.SizeInBytes);
        Stack2DContent.SetData(Stack2DContentData);

        NewElement = new ComputeBuffer(1, Paint.SizeInBytes);
        NewElement.SetData(NewElementData);

        Attributes.Add(new CSComputeBuffer("Stack2DInfo", Stack2DInfo));
        Attributes.Add(new CSComputeBuffer("Stack2DContent", Stack2DContent));
        Attributes.Add(new CSInt2("Stack2DSize", stack2DSize));
        Attributes.Add(new CSInt2("NewElementPosition", newElementPosition));
        Attributes.Add(new CSComputeBuffer("NewElement", NewElement));

        ComputeShaderTask cst = new ComputeShaderTask(
            "Tests/TestStack",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            Attributes,
            true,
            kernelID);

        cst.Run();

        Stack2DContent.GetData(Stack2DContentData);
        Stack2DInfo.GetData(Stack2DInfoData);

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
    public void empty_element_does_nothing()
    {
        // Arrange
        Stack2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 0, MaxSize = 1, WriteIndex = 0, Volume = 0 }
        };
        Stack2DContentData = new Paint[]
        {
            P(0),
        };
        NewElementData = new Paint[]
        {
            P(0)
        };


        // Act
        Execute(KERNEL_ID_raw_push);


        // Assert
        Assert.AreEqual(
            new StackInfo[]
            {
                new StackInfo { Size = 0, MaxSize = 1, WriteIndex = 0, Volume = 0 }
            },
            Stack2DInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0)
            },
            Stack2DContentData);
    }

    [Test]
    public void voxel_empty_enough_space()
    {
        // Arrange
        Stack2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 0, MaxSize = 1, WriteIndex = 0, Volume = 0 }
        };
        Stack2DContentData = new Paint[]
        {
            P(-1),
        };
        NewElementData = new Paint[]
        {
            P(0.6f)
        };


        // Act
        Execute(KERNEL_ID_raw_push);


        // Assert
        Assert.AreEqual(
            new StackInfo[]
            {
                new StackInfo { Size = 1, MaxSize = 1, WriteIndex = 1, Volume = 0.6f }
            },
            Stack2DInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0.6f),
            },
            Stack2DContentData);
    }

    [Test]
    public void voxel_filled_enough_space()
    {
        // Arrange
        Stack2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 1, MaxSize = 2, WriteIndex = 1, Volume = 1 }
        };
        Stack2DContentData = new Paint[]
        {
            P( 1),

            P(-1),
        };
        NewElementData = new Paint[]
        {
            P(0.6f)
        };


        // Act
        Execute(KERNEL_ID_raw_push);


        // Assert
        Assert.AreEqual(
            new StackInfo[]
            {
                new StackInfo { Size = 2, MaxSize = 2, WriteIndex = 2, Volume = 1.6f }
            },
            Stack2DInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P( 1),

                P(0.6f),
            },
            Stack2DContentData);
    }

    [Test]
    public void voxel_filled_not_enough_space_does_nothing()
    {
        Stack2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 1, MaxSize = 1, WriteIndex = 1, Volume = 1 },
        };
        Stack2DContentData = new Paint[]
        {
            P(1),
        };
        NewElementData = new Paint[]
        {
            P(0.6f)
        };


        // Act
        Execute(KERNEL_ID_raw_push);


        // Assert
        Assert.AreEqual(
            new StackInfo[]
            {
                new StackInfo { Size = 1, MaxSize = 1, WriteIndex = 1, Volume = 1 },
            },
            Stack2DInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(1),
            },
            Stack2DContentData);
    }

    [Test]
    public void voxel_half_filled_enough_space()
    {
        // Arrange
        Stack2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 1, MaxSize = 2, WriteIndex = 1, Volume = 0.5f }
        };
        Stack2DContentData = new Paint[]
        {
            P(1, 0.5f),

            P(-1),
        };
        NewElementData = new Paint[]
        {
            P(1, 0.5f)
        };


        // Act
        Execute(KERNEL_ID_raw_push);


        // Assert
        Assert.AreEqual(
            new StackInfo[]
            {
            new StackInfo { Size = 2, MaxSize = 2, WriteIndex = 2, Volume = 1 }
            },
            Stack2DInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(1, 0.5f),

                P(1, 0.5f),
            },
            Stack2DContentData);
    }

    [Test]
    public void voxel_half_filled_not_enough_space()
    {
        // Arrange
        Stack2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 1, MaxSize = 1, WriteIndex = 1, Volume = 0.2f }
        };
        Stack2DContentData = new Paint[]
        {
            P(1, 0.2f),
        };
        NewElementData = new Paint[]
        {
            P(1, 0.5f)
        };


        // Act
        Execute(KERNEL_ID_raw_push);


        // Assert
        Assert.AreEqual(
            new StackInfo[]
            {
            new StackInfo { Size = 1, MaxSize = 1, WriteIndex = 1, Volume = 0.2f }
            },
            Stack2DInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(1, 0.2f),
            },
            Stack2DContentData);
    }
}