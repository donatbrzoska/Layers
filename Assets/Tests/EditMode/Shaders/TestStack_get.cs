using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestStack_get
{
    private const int KERNEL_ID_get = 5;

    ComputeBuffer Stack2DInfo;
    StackInfo[] Stack2DInfoData;
    ComputeBuffer Stack2DContent;
    Paint[] Stack2DContentData;
    Vector2Int Stack2DSize;
    Vector3Int GetPosition;
    ComputeBuffer GetResult;
    Paint[] GetResultData;

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
        GetResult.Dispose();

        new FileLogger_().OnDisable();
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        Stack2DInfo = new ComputeBuffer(Stack2DSize.x * Stack2DSize.y, StackInfo.SizeInBytes);
        Stack2DInfo.SetData(Stack2DInfoData);

        Stack2DContent = new ComputeBuffer(Stack2DSize.x * Stack2DSize.y * Stack2DInfoData[0].MaxSize, Paint.SizeInBytes);
        Stack2DContent.SetData(Stack2DContentData);

        GetResult = new ComputeBuffer(1, Paint.SizeInBytes);
        GetResultData = new Paint[] { P(-1) };
        GetResult.SetData(GetResultData);

        List<CSAttribute> Attributes = new List<CSAttribute>();
        Attributes.Add(new CSComputeBuffer("Stack2DInfo", Stack2DInfo));
        Attributes.Add(new CSComputeBuffer("Stack2DContent", Stack2DContent));
        Attributes.Add(new CSInt2("Stack2DSize", Stack2DSize));
        Attributes.Add(new CSInt3("GetPosition", GetPosition));
        Attributes.Add(new CSComputeBuffer("GetResult", GetResult));

        ComputeShaderTask cst = new ComputeShaderTask(
            "Tests/TestStack",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            Attributes,
            true,
            kernelID);

        cst.Run();

        Stack2DContent.GetData(Stack2DContentData);
        Stack2DInfo.GetData(Stack2DInfoData);
        GetResult.GetData(GetResultData);

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
    public void empty_element_is_returned_as_is()
    {
        // Arrange
        Stack2DInfoData = new StackInfo[]
        {
            new StackInfo { Size = 0, MaxSize = 1, WriteIndex = 0, Volume = 0 }
        };
        Stack2DContentData = new Paint[]
        {
            P(0.3f, 0),
        };
        GetPosition = Vector3Int.zero;


        // Act
        Execute(KERNEL_ID_get);


        // Assert
        Assert.AreEqual(
            P(0.3f, 0),
            GetResultData[0]);
    }

    [Test]
    public void get_correct_position_also()
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
        GetPosition = new Vector3Int(1, 2, 1);


        // Act
        Execute(KERNEL_ID_get);


        // Assert
        Assert.AreEqual(
            P(0.4f, 0.2f),
            GetResultData[0]);
    }

    // Test: Out of range? Should never happen though ...
}