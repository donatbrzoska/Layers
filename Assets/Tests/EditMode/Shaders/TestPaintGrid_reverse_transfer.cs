using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestPaintGrid_reverse_transfer
{
    private const int KERNEL_ID_reverse_tranfer = 3;

    List<CSAttribute> Attributes;

    ComputeBuffer SourcePaintGridInfo;
    ColumnInfo[] SourcePaintGridInfoData;
    ComputeBuffer SourcePaintGridContent;
    Paint[] SourcePaintGridContentData;
    Vector3Int SourcePaintGridSize;

    ComputeBuffer TargetPaintGridInfo;
    ColumnInfo[] TargetPaintGridInfoData;
    ComputeBuffer TargetPaintGridContent;
    Paint[] TargetPaintGridContentData;
    Vector3Int TargetPaintGridSize;
    Vector2Int TargetPaintGridPosition;


    [SetUp]
    public void Setup()
    {
        Attributes = new List<CSAttribute>();

        new FileLogger_().OnEnable();
    }

    [TearDown]
    public void Teardown()
    {
        SourcePaintGridInfo.Dispose();
        SourcePaintGridContent.Dispose();
        TargetPaintGridInfo.Dispose();
        TargetPaintGridContent.Dispose();

        new FileLogger_().OnDisable();
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        SourcePaintGridInfo = new ComputeBuffer(SourcePaintGridSize.x * SourcePaintGridSize.y, ColumnInfo.SizeInBytes);
        SourcePaintGridInfo.SetData(SourcePaintGridInfoData);
        SourcePaintGridContent = new ComputeBuffer(SourcePaintGridSize.x * SourcePaintGridSize.y * SourcePaintGridSize.z, Paint.SizeInBytes);
        SourcePaintGridContent.SetData(SourcePaintGridContentData);

        TargetPaintGridInfo = new ComputeBuffer(TargetPaintGridSize.x * TargetPaintGridSize.y, ColumnInfo.SizeInBytes);
        TargetPaintGridInfo.SetData(TargetPaintGridInfoData);
        TargetPaintGridContent = new ComputeBuffer(TargetPaintGridSize.x * TargetPaintGridSize.y * TargetPaintGridSize.z, Paint.SizeInBytes);
        TargetPaintGridContent.SetData(TargetPaintGridContentData);

        Attributes.Add(new CSComputeBuffer("SourcePGInfo", SourcePaintGridInfo));
        Attributes.Add(new CSComputeBuffer("SourcePGContent", SourcePaintGridContent));
        Attributes.Add(new CSInt3("SourcePGSize", SourcePaintGridSize));

        Attributes.Add(new CSComputeBuffer("TargetPGInfo", TargetPaintGridInfo));
        Attributes.Add(new CSComputeBuffer("TargetPGContent", TargetPaintGridContent));
        Attributes.Add(new CSInt3("TargetPGSize", TargetPaintGridSize));
        Attributes.Add(new CSFloat("TargetPGCellVolume", 1));
        Attributes.Add(new CSInt2("TargetPGPosition", TargetPaintGridPosition));

        ComputeShaderTask cst = new ComputeShaderTask(
            "Tests/TestPaintGrid",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            Attributes,
            true,
            kernelID);

        cst.Run();

        TargetPaintGridContent.GetData(TargetPaintGridContentData);
        TargetPaintGridInfo.GetData(TargetPaintGridInfoData);

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
    public void empty_source_column_does_nothing()
    {
        // Arrange
        SourcePaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }
        };
        SourcePaintGridContentData = new Paint[]
        {
            P(0),
        };
        SourcePaintGridSize = new Vector3Int(1, 1, 1);

        TargetPaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }
        };
        TargetPaintGridContentData = new Paint[]
        {
            P(-1),
        };
        TargetPaintGridSize = new Vector3Int(1, 1, 1);
        TargetPaintGridPosition = Vector2Int.zero;


        // Act
        Execute(KERNEL_ID_reverse_tranfer);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }
            },
            TargetPaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(-1)
            },
            TargetPaintGridContentData);
    }

    [Test]
    public void correct_position()
    {
        // Arrange
        SourcePaintGridInfoData = new ColumnInfo[]
        {
            // Notice that write index is 1 because we simulate a preceding raw_push
            new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 0.6f } 
        };
        SourcePaintGridContentData = new Paint[]
        {
            P(0.6f),
        };
        SourcePaintGridSize = new Vector3Int(1, 1, 1);

        TargetPaintGridInfoData = new ColumnInfo[]
        {
                new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }, new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 },
                new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }, new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 },
                new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }, new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 },
        };
        TargetPaintGridContentData = new Paint[]
        {
            P(-1), P(-1),
            P(-1), P(-1),
            P(-1), P(-1)
        };
        TargetPaintGridSize = new Vector3Int(2, 3, 1);
        TargetPaintGridPosition = new Vector2Int(1, 2);


        // Act
        Execute(KERNEL_ID_reverse_tranfer);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }, new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 },
                new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }, new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 },
                new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }, new ColumnInfo { Size = 1, WriteIndex = 0, Volume = 0.6f }
            },
            TargetPaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(-1), P(-1),
                P(-1), P(-1),
                P(-1), P(0.6f)
            },
            TargetPaintGridContentData);
    }

    [Test]
    public void correct_mix_in()
    {
        // Arrange
        SourcePaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 0.8f }
        };
        SourcePaintGridContentData = new Paint[]
        {
            P(0, 0.6f),
        };
        SourcePaintGridSize = new Vector3Int(1, 1, 1);

        TargetPaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 2, WriteIndex = 1, Volume = 1.2f }
        };
        TargetPaintGridContentData = new Paint[]
        {
            P(1),

            P(1, 0.2f),
        };
        TargetPaintGridSize = new Vector3Int(1, 1, 2);
        TargetPaintGridPosition = Vector2Int.zero;


        // Act
        Execute(KERNEL_ID_reverse_tranfer);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 2, WriteIndex = 1, Volume = 1.8f }
            },
            TargetPaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(1),

                P(0.25f, 0.8f),
            },
            TargetPaintGridContentData);
    }

    [Test]
    public void multiple_elements()
    {
        // Arrange
        SourcePaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 2, WriteIndex = 2, Volume = 1.6f }
        };
        SourcePaintGridContentData = new Paint[]
        {
            P(1, 0.6f),

            P(0, 1),
        };
        SourcePaintGridSize = new Vector3Int(1, 1, 2);

        TargetPaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }
        };
        TargetPaintGridContentData = new Paint[]
        {
            P(-1),

            P(-1),
        };
        TargetPaintGridSize = new Vector3Int(1, 1, 2);
        TargetPaintGridPosition = Vector2Int.zero;


        // Act
        Execute(KERNEL_ID_reverse_tranfer);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 2, WriteIndex = 1, Volume = 1.6f }
            },
            TargetPaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0.6f, 1),

                P(0, 0.6f),
            },
            TargetPaintGridContentData);
    }
}