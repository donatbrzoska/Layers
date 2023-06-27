using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestPaintGrid_delete
{
    private const int KERNEL_ID_delete = 4;

    ComputeBuffer PaintGridInfo;
    ColumnInfo[] PaintGridInfoData;
    ComputeBuffer PaintGridContent;
    Paint[] PaintGridContentData;
    Vector3Int PaintGridSize;
    Vector3Int DeletePosition;
    float DeleteVolume;

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

        new FileLogger_().OnDisable();
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        PaintGridInfo?.Dispose();
        PaintGridInfo = new ComputeBuffer(PaintGridSize.x * PaintGridSize.y, ColumnInfo.SizeInBytes);
        PaintGridInfo.SetData(PaintGridInfoData);

        PaintGridContent?.Dispose();
        PaintGridContent = new ComputeBuffer(PaintGridSize.x * PaintGridSize.y * PaintGridSize.z, Paint.SizeInBytes);
        PaintGridContent.SetData(PaintGridContentData);

        List<CSAttribute> Attributes = new List<CSAttribute>();
        Attributes.Add(new CSComputeBuffer("PaintGridInfo", PaintGridInfo));
        Attributes.Add(new CSComputeBuffer("PaintGridContent", PaintGridContent));
        Attributes.Add(new CSInt3("PaintGridSize", PaintGridSize));
        Attributes.Add(new CSInt3("DeletePosition", DeletePosition));
        Attributes.Add(new CSFloat("DeleteVolume", DeleteVolume));

        ComputeShaderTask cst = new ComputeShaderTask(
            "Tests/TestPaintGrid",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            Attributes,
            true,
            kernelID);

        cst.Run();

        PaintGridContent.GetData(PaintGridContentData);
        PaintGridInfo.GetData(PaintGridInfoData);

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
    public void empty_column_does_nothing()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }
        };
        PaintGridContentData = new Paint[]
        {
            P(0),
        };
        PaintGridSize = new Vector3Int(1, 1, 1);
        DeletePosition = Vector3Int.zero;
        DeleteVolume = 0.4f;


        // Act
        Execute(KERNEL_ID_delete);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0)
            },
            PaintGridContentData);
    }

    [Test]
    public void delete_correct_position_also()
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
        DeletePosition = new Vector3Int(1, 2, 1);
        DeleteVolume = 0.1f;


        // Act
        Execute(KERNEL_ID_delete);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1 }, new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1 },
                new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1 }, new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1 },
                new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1 }, new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1.1f },
            },
            PaintGridInfoData);

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
            PaintGridContentData);
    }

    [Test]
    public void usual_delete_complete_cell_or_more()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 2, WriteIndex = 1, Volume = 1.3f }
        };
        PaintGridContentData = new Paint[]
        {
            P(1),

            P(0.3f),
        };
        PaintGridSize = new Vector3Int(1, 1, 2);
        DeletePosition = new Vector3Int(0, 0, 1);
        DeleteVolume = 0.4f;


        // Act
        Execute(KERNEL_ID_delete);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1 }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(1),

                P(0.3f, 0), // Notice that the color stays, doesn't matter because there is zero volume
            },
            PaintGridContentData);
    }

    [Test]
    public void delete_complete_lowest_cell()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 1, WriteIndex = 0, Volume = 0.3f }
        };
        PaintGridContentData = new Paint[]
        {
            P(0.3f),
        };
        PaintGridSize = new Vector3Int(1, 1, 1);
        DeletePosition = Vector3Int.zero;
        DeleteVolume = 0.3f;


        // Act
        Execute(KERNEL_ID_delete);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0.3f, 0) // Notice that the color stays, doesn't matter because there is zero volume
            },
            PaintGridContentData);
    }

    [Test]
    public void delete_from_basically_empty_column()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0.00001f }
        };
        PaintGridContentData = new Paint[]
        {
            P(0.3f, 0.00001f),
        };
        PaintGridSize = new Vector3Int(1, 1, 1);
        DeletePosition = Vector3Int.zero;
        DeleteVolume = 0.1f;


        // Act
        Execute(KERNEL_ID_delete);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0.00001f }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0.3f, 0), // Notice that the color stays - doesn't matter though, because there is zero volume
            },
            PaintGridContentData);
    }

    [Test]
    public void auto_update_write_index()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 2, WriteIndex = 1, Volume = 1.3f }
        };
        PaintGridContentData = new Paint[]
        {
            P(0.2f, 1),

            P(0.2f, 0.3f),
        };
        PaintGridSize = new Vector3Int(1, 1, 2);


        // Act
        DeletePosition = new Vector3Int(0, 0, 1);
        DeleteVolume = 0.4f;
        Execute(KERNEL_ID_delete);

        DeletePosition.z--;
        DeleteVolume = 1;
        Execute(KERNEL_ID_delete);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0.2f, 0), // Notice that the color stays, doesn't matter because there is zero volume

                P(0.2f, 0), // Notice that the color stays, doesn't matter because there is zero volume
            },
            PaintGridContentData);
    }

    // Test: delete cell at write_index is not filled -> shouldn't matter because we don't use the stack functionality for delete
}