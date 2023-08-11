using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestPaintGrid_get
{
    private const int KERNEL_ID_get = 5;

    ComputeBuffer PaintGridInfo;
    ColumnInfo[] PaintGridInfoData;
    ComputeBuffer PaintGridContent;
    Paint[] PaintGridContentData;
    Vector3Int PaintGridSize;
    Vector3Int GetPosition;
    ComputeBuffer GetResult;
    Paint[] GetResultData;

    [SetUp]
    public void Setup()
    {
        new FileLogger_().OnEnable();
    }

    [TearDown]
    public void Teardown()
    {
        PaintGridInfo.Dispose();
        PaintGridContent.Dispose();
        GetResult.Dispose();

        new FileLogger_().OnDisable();
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        PaintGridInfo = new ComputeBuffer(PaintGridSize.x * PaintGridSize.y, ColumnInfo.SizeInBytes);
        PaintGridInfo.SetData(PaintGridInfoData);

        PaintGridContent = new ComputeBuffer(PaintGridSize.x * PaintGridSize.y * PaintGridSize.z, Paint.SizeInBytes);
        PaintGridContent.SetData(PaintGridContentData);

        GetResult = new ComputeBuffer(1, Paint.SizeInBytes);
        GetResultData = new Paint[] { P(-1) };
        GetResult.SetData(GetResultData);

        List<CSAttribute> Attributes = new List<CSAttribute>();
        Attributes.Add(new CSComputeBuffer("PaintGridInfo", PaintGridInfo));
        Attributes.Add(new CSComputeBuffer("PaintGridContent", PaintGridContent));
        Attributes.Add(new CSInt3("PaintGridSize", PaintGridSize));
        Attributes.Add(new CSInt3("GetPosition", GetPosition));
        Attributes.Add(new CSComputeBuffer("GetResult", GetResult));

        ComputeShaderTask cst = new ComputeShaderTask(
            "Tests/TestPaintGrid",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            Attributes,
            true,
            kernelID);

        cst.Run();

        PaintGridContent.GetData(PaintGridContentData);
        PaintGridInfo.GetData(PaintGridInfoData);
        GetResult.GetData(GetResultData);

        return cst;
    }

    Paint P(float v)
    {
        return new Paint(new Vector3(v, v, v), v);
    }

    Paint P(float color, float volume)
    {
        return new Paint(new Vector3(color, color, color), volume);
    }

    [Test]
    public void empty_element_is_returned_as_is()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }
        };
        PaintGridContentData = new Paint[]
        {
            P(0.3f, 0),
        };
        PaintGridSize = new Vector3Int(1, 1, 1);
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
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1 }, new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1 },
            new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1 }, new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1 },
            new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1 }, new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1.2f },
        };
        PaintGridContentData = new Paint[]
        {
            P(1), P(1),
            P(1), P(1),
            P(1), P(1),

            P(-1), P(-1),
            P(-1), P(-1),
            P(-1), P(0.4f, 0.2f),
        };
        PaintGridSize = new Vector3Int(2, 3, 2);
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