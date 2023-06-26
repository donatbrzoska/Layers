using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestStack_delete
{
    private const int KERNEL_ID_delete = 4;

    ComputeBuffer Stack2DInfo;
    StackInfo[] Stack2DInfoData;
    ComputeBuffer Stack2DContent;
    Paint[] Stack2DContentData;
    Vector2Int Stack2DSize;
    Vector3Int DeletePosition;
    float DeleteVolume;

    [SetUp]
    public void Setup()
    {
        Stack2DSize = new Vector2Int(1, 1);

        new FileLogger_().OnEnable();
    }

    [TearDown]
    public void Teardown()
    {
        Stack2DInfo.Dispose();
        Stack2DContent.Dispose();

        new FileLogger_().OnDisable();
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        Stack2DInfo?.Dispose();
        Stack2DInfo = new ComputeBuffer(Stack2DSize.x * Stack2DSize.y, StackInfo.SizeInBytes);
        Stack2DInfo.SetData(Stack2DInfoData);

        Stack2DContent?.Dispose();
        Stack2DContent = new ComputeBuffer(Stack2DSize.x * Stack2DSize.y * Stack2DInfoData[0].MaxSize, Paint.SizeInBytes);
        Stack2DContent.SetData(Stack2DContentData);

        List<CSAttribute> Attributes = new List<CSAttribute>();
        Attributes.Add(new CSComputeBuffer("Stack2DInfo", Stack2DInfo));
        Attributes.Add(new CSComputeBuffer("Stack2DContent", Stack2DContent));
        Attributes.Add(new CSInt2("Stack2DSize", Stack2DSize));
        Attributes.Add(new CSInt3("DeletePosition", DeletePosition));
        Attributes.Add(new CSFloat("DeleteVolume", DeleteVolume));

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
    public void empty_stack_does_nothing()
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
        DeletePosition = Vector3Int.zero;
        DeleteVolume = 0.4f;


        // Act
        Execute(KERNEL_ID_delete);


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
    public void delete_correct_position_also()
    {
        int MS = 2;

        // Arrange
        Stack2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 1, MaxSize = MS, WriteIndex = 1, Volume = 1 }, new StackInfo { Size = 1, MaxSize = MS, WriteIndex = 1, Volume = 1 },
            new StackInfo { Size = 1, MaxSize = MS, WriteIndex = 1, Volume = 1 }, new StackInfo { Size = 1, MaxSize = MS, WriteIndex = 1, Volume = 1 },
            new StackInfo { Size = 1, MaxSize = MS, WriteIndex = 1, Volume = 1 }, new StackInfo { Size = 1, MaxSize = MS, WriteIndex = 1, Volume = 1.2f },
        };
        Stack2DContentData = new Paint[]
        {
            P(1), P(1),
            P(1), P(1),
            P(1), P(1),

            P(-1), P(-1),
            P(-1), P(-1),
            P(-1), P(0.4f, 0.2f),
        };
        Stack2DSize = new Vector2Int(2, 3);
        DeletePosition = new Vector3Int(1, 2, 1);
        DeleteVolume = 0.1f;


        // Act
        Execute(KERNEL_ID_delete);


        // Assert
        Assert.AreEqual(
            new StackInfo[]
            {
                new StackInfo { Size = 1, MaxSize = MS, WriteIndex = 1, Volume = 1 }, new StackInfo { Size = 1, MaxSize = MS, WriteIndex = 1, Volume = 1 },
                new StackInfo { Size = 1, MaxSize = MS, WriteIndex = 1, Volume = 1 }, new StackInfo { Size = 1, MaxSize = MS, WriteIndex = 1, Volume = 1 },
                new StackInfo { Size = 1, MaxSize = MS, WriteIndex = 1, Volume = 1 }, new StackInfo { Size = 1, MaxSize = MS, WriteIndex = 1, Volume = 1.1f },
            },
            Stack2DInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                 P(1),  P(1),
                 P(1),  P(1),
                 P(1),  P(1),

                P(-1), P(-1),
                P(-1), P(-1),
                P(-1),  P(0.4f, 0.1f),
            },
            Stack2DContentData);
    }

    [Test]
    public void usual_delete_complete_cell_or_more()
    {
        // Arrange
        Stack2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 2, MaxSize = 2, WriteIndex = 1, Volume = 1.3f }
        };
        Stack2DContentData = new Paint[]
        {
            P(1),

            P(0.3f),
        };
        DeletePosition = new Vector3Int(0, 0, 1);
        DeleteVolume = 0.4f;


        // Act
        Execute(KERNEL_ID_delete);


        // Assert
        Assert.AreEqual(
            new StackInfo[]
            {
                new StackInfo { Size = 1, MaxSize = 2, WriteIndex = 1, Volume = 1 }
            },
            Stack2DInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(1),

                P(0.3f, 0), // Notice that the color stays, doesn't matter because there is zero volume
            },
            Stack2DContentData);
    }

    [Test]
    public void delete_complete_lowest_cell()
    {
        // Arrange
        Stack2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 1, MaxSize = 1, WriteIndex = 0, Volume = 0.3f }
        };
        Stack2DContentData = new Paint[]
        {
            P(0.3f),
        };
        DeletePosition = Vector3Int.zero;
        DeleteVolume = 0.3f;


        // Act
        Execute(KERNEL_ID_delete);


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
                P(0.3f, 0) // Notice that the color stays, doesn't matter because there is zero volume
            },
            Stack2DContentData);
    }

    [Test]
    public void auto_update_write_index()
    {
        // Arrange
        Stack2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 2, MaxSize = 2, WriteIndex = 1, Volume = 1.3f }
        };
        Stack2DContentData = new Paint[]
        {
            P(0.2f, 1),

            P(0.2f, 0.3f),
        };


        // Act
        DeletePosition = new Vector3Int(0, 0, 1);
        DeleteVolume = 0.4f;
        Execute(KERNEL_ID_delete);

        DeletePosition.z--;
        DeleteVolume = 1;
        Execute(KERNEL_ID_delete);


        // Assert
        Assert.AreEqual(
            new StackInfo[]
            {
                new StackInfo { Size = 0, MaxSize = 2, WriteIndex = 0, Volume = 0 }
            },
            Stack2DInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0.2f, 0), // Notice that the color stays, doesn't matter because there is zero volume

                P(0.2f, 0), // Notice that the color stays, doesn't matter because there is zero volume
            },
            Stack2DContentData);
    }

    // Test: delete cell at write_index is not filled -> shouldn't matter because we don't use the stack functionality for delete
}