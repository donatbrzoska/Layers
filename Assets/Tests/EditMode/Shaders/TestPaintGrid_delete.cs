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
        Attributes.Add(new CSFloat("PaintGridCellVolume", 1));
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
        return new Paint(new Vector3(v, v, v), v);
    }

    Paint P(float color, float volume)
    {
        return new Paint(new Vector3(color, color, color), volume);
    }

    Paint P0 = new Paint(new Vector3(0, 0, 0), 0);

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
            P0
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
                P0
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
            new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1 }, new ColumnInfo { Size = 2, WriteIndex = 1, Volume = 1.2f },
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
                new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1 }, new ColumnInfo { Size = 2, WriteIndex = 1, Volume = 1.1f },
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

                P0
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
                P0
            },
            PaintGridContentData);
    }

    [Test]
    public void delete_from_almost_empty_column()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 1, WriteIndex = 0, Volume = 0.01f }
        };
        PaintGridContentData = new Paint[]
        {
            P(0.3f, 0.01f),
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
                new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P0
            },
            PaintGridContentData);
    }

    float FLOAT_PRECISION = 0.01f;

    [Test]
    public void delete_from_almost_empty_column_drop_below_precision()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 1, WriteIndex = 0, Volume = FLOAT_PRECISION + 0.1f * FLOAT_PRECISION }
        };
        PaintGridContentData = new Paint[]
        {
            P(0.3f, FLOAT_PRECISION + 0.1f * FLOAT_PRECISION),
        };
        PaintGridSize = new Vector3Int(1, 1, 1);
        DeletePosition = Vector3Int.zero;
        DeleteVolume = 0.2f * FLOAT_PRECISION;


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
                P0
            },
            PaintGridContentData);
    }

    [Test]
    public void delete_from_empty_column_above_zero_though()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0.5f * FLOAT_PRECISION }
        };
        PaintGridContentData = new Paint[]
        {
            P(0.3f, 0.5f * FLOAT_PRECISION),
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
                new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P0
            },
            PaintGridContentData);
    }

    [Test]
    public void auto_update_write_index_full_delete()
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
                P0,

                P0,
            },
            PaintGridContentData);
    }

    [Test]
    public void auto_update_write_index_partial_delete()
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
        DeleteVolume = 0.1f;
        Execute(KERNEL_ID_delete);

        DeletePosition.z--;
        DeleteVolume = 0.1f;
        Execute(KERNEL_ID_delete);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 2, WriteIndex = 0, Volume = 1.1f }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0.2f, 0.9f),

                P(0.2f, 0.2f),
            },
            PaintGridContentData);
    }

    [Test]
    public void auto_update_write_index_not_filled_below_impossible_write_index_its_possible_though_lol()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 2, WriteIndex = 1, Volume = 0.2f }
        };
        PaintGridContentData = new Paint[]
        {
            P(0.2f, 0.1f),

            P(0.2f, 0.1f),
        };
        PaintGridSize = new Vector3Int(1, 1, 2);


        // Act
        DeletePosition = new Vector3Int(0, 0, 1);
        DeleteVolume = 0.1f;
        Execute(KERNEL_ID_delete);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 1, WriteIndex = 0, Volume = 0.1f }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0.2f, 0.1f),

                P0,
            },
            PaintGridContentData);
    }

    [Test]
    public void update_size_empty()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 1, WriteIndex = 0, Volume = 0.1f }
        };
        PaintGridContentData = new Paint[]
        {
            P(0.2f, 0.1f),

            P0,
        };
        PaintGridSize = new Vector3Int(1, 1, 2);


        // Act
        DeletePosition = new Vector3Int(0, 0, 0);
        DeleteVolume = 0.1f;
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
                P0,

                P0,
            },
            PaintGridContentData);
    }

    [Test]
    public void update_size_one_left()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 3, WriteIndex = 0, Volume = 0.3f }
        };
        PaintGridContentData = new Paint[]
        {
            P(0.2f, 0.1f),

            P(0.2f, 0.1f),

            P(0.2f, 0.1f),
        };
        PaintGridSize = new Vector3Int(1, 1, 3);


        // Act
        DeletePosition = new Vector3Int(0, 0, 2);
        DeleteVolume = 0.1f;
        Execute(KERNEL_ID_delete);
        DeletePosition = new Vector3Int(0, 0, 1);
        DeleteVolume = 0.1f;
        Execute(KERNEL_ID_delete);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 1, WriteIndex = 0, Volume = 0.1f }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0.2f, 0.1f),

                P0,

                P0,
            },
            PaintGridContentData);
    }

    // partial delete very little volume doesn't really matter since size is
    // .. not decreased without emptying and fill will just fill up the very little

    // Test: delete cell at write_index is not filled -> shouldn't matter because we don't use the stack functionality for delete
}