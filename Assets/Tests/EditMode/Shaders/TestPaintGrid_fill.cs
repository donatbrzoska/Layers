using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestPaintGrid_fill
{
    private const int KERNEL_ID_fill = 1;

    List<CSAttribute> Attributes;

    ComputeBuffer PaintGridInfo;
    ColumnInfo[] PaintGridInfoData;
    ComputeBuffer PaintGridContent;
    Paint[] PaintGridContentData;
    Vector3Int PaintGridSize;
    float PaintGridCellVolume;
    int PaintGridDiffuseDepth;
    float PaintGridDiffuseRatio;
    Vector2Int NewElementPosition;
    ComputeBuffer NewElement;
    Paint[] NewElementData;

    [SetUp]
    public void Setup()
    {
        Attributes = new List<CSAttribute>();

        PaintGridCellVolume = 1;
        PaintGridDiffuseDepth = 0;
        PaintGridDiffuseRatio = 0;
        NewElementPosition = Vector2Int.zero;

        new FileLogger_().OnEnable();
    }

    [TearDown]
    public void Teardown()
    {
        PaintGridInfo.Dispose();
        PaintGridContent.Dispose();
        NewElement.Dispose();

        new FileLogger_().OnDisable();
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        PaintGridInfo = new ComputeBuffer(PaintGridSize.x * PaintGridSize.y, ColumnInfo.SizeInBytes);
        PaintGridInfo.SetData(PaintGridInfoData);

        PaintGridContent = new ComputeBuffer(PaintGridSize.x * PaintGridSize.y * PaintGridSize.z, Paint.SizeInBytes);
        PaintGridContent.SetData(PaintGridContentData);

        NewElement = new ComputeBuffer(1, Paint.SizeInBytes);
        NewElement.SetData(NewElementData);

        Attributes.Add(new CSComputeBuffer("PaintGridInfo", PaintGridInfo));
        Attributes.Add(new CSComputeBuffer("PaintGridContent", PaintGridContent));
        Attributes.Add(new CSInt3("PaintGridSize", PaintGridSize));
        Attributes.Add(new CSFloat("PaintGridCellVolume", PaintGridCellVolume));
        Attributes.Add(new CSInt("PaintGridDiffuseDepth", PaintGridDiffuseDepth));
        Attributes.Add(new CSFloat("PaintGridDiffuseRatio", PaintGridDiffuseRatio));
        Attributes.Add(new CSInt2("NewElementPosition", NewElementPosition));
        Attributes.Add(new CSComputeBuffer("NewElement", NewElement));

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

    [Test]
    public void voxel_empty_enough_space_correct_position_also()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }, new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 },
            new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }, new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 },
            new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }, new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 },
        };
        PaintGridContentData = new Paint[]
        {
            P(0), P(0),
            P(0), P(0),
            P(0), P(0)
        };
        PaintGridSize = new Vector3Int(2, 3, 1);
        NewElementPosition = new Vector2Int(1, 2);
        NewElementData = new Paint[]
        {
            P(0.6f)
        };


        // Act
        Execute(KERNEL_ID_fill);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }, new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 },
                new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }, new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 },
                new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }, new ColumnInfo { Size = 1, WriteIndex = 0, Volume = 0.6f }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0), P(0),
                P(0), P(0),
                P(0), P(0.6f)
            },
            PaintGridContentData);
    }

    [Test]
    public void empty_element_does_nothing()
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
        PaintGridSize = new Vector3Int(2, 3, 1);
        NewElementData = new Paint[]
        {
            P(0)
        };


        // Act
        Execute(KERNEL_ID_fill);


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
    public void voxel_filled_enough_space()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1 }
        };
        PaintGridContentData = new Paint[]
        {
            P( 1),

            P(-1),
        };
        PaintGridSize = new Vector3Int(1, 1, 2);
        NewElementData = new Paint[]
        {
            P(0.6f)
        };


        // Act
        Execute(KERNEL_ID_fill);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 2, WriteIndex = 1, Volume = 1.6f }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P( 1),

                P(0.6f),
            },
            PaintGridContentData);
    }

    [Test] //?
    public void voxel_filled_enough_space_add_more_than_cell_volume()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1 }
        };
        PaintGridContentData = new Paint[]
        {
            P( 1),

            P(0),

            P(0),
        };
        PaintGridSize = new Vector3Int(1, 1, 3);
        NewElementData = new Paint[]
        {
            P(0.6f, 2)
        };


        // Act
        Execute(KERNEL_ID_fill);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 3, WriteIndex = 3, Volume = 3 }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P( 1),

                P(0.6f, 1),

                P(0.6f, 1),
            },
            PaintGridContentData);
    }


    [Test]
    public void voxel_filled_not_enough_space_does_nothing()
    {
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1 },
        };
        PaintGridContentData = new Paint[]
        {
            P(1),
        };
        PaintGridSize = new Vector3Int(1, 1, 1);
        NewElementData = new Paint[]
        {
            P(0.6f)
        };


        // Act
        Execute(KERNEL_ID_fill);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1 },
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(1),
            },
            PaintGridContentData);
    }

    [Test]
    public void voxel_half_filled_exact_fill_enough_space()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 2, WriteIndex = 1, Volume = 1.5f }
        };
        PaintGridContentData = new Paint[]
        {
            P(1, 1),

            P(0, 0.5f),
        };
        PaintGridSize = new Vector3Int(1, 1, 2);
        NewElementData = new Paint[]
        {
            P(1, 0.5f)
        };


        // Act
        Execute(KERNEL_ID_fill);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 2, WriteIndex = 2, Volume = 2 }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(1, 1),

                P(0.5f, 1),
            },
            PaintGridContentData);
    }

    [Test]
    public void voxel_half_filled_partial_fill_enough_space()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 1, WriteIndex = 0, Volume = 0.2f }
        };
        PaintGridContentData = new Paint[]
        {
            P(0, 0.2f),
        };
        PaintGridSize = new Vector3Int(1, 1, 1);
        NewElementData = new Paint[]
        {
            P(1, 0.6f)
        };


        // Act
        Execute(KERNEL_ID_fill);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 1, WriteIndex = 0, Volume = 0.8f }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0.75f, 0.8f),
            },
            PaintGridContentData);
    }

    [Test]
    public void voxel_half_filled_spanning_fill_enough_space()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 1, WriteIndex = 0, Volume = 0.5f }
        };
        PaintGridContentData = new Paint[]
        {
            P(0, 0.5f),

            P(-1),
        };
        PaintGridSize = new Vector3Int(1, 1, 2);
        NewElementData = new Paint[]
        {
            P(1, 1)
        };


        // Act
        Execute(KERNEL_ID_fill);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 2, WriteIndex = 1, Volume = 1.5f }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0.5f, 1),

                P(1, 0.5f),
            },
            PaintGridContentData);
    }

    [Test] //?
    public void voxel_half_filled_spanning_fill_enough_space_add_more_than_cell_volume()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 1, WriteIndex = 0, Volume = 0.5f }
        };
        PaintGridContentData = new Paint[]
        {
            P(0, 0.5f),

            P(0),

            P(0),
        };
        PaintGridSize = new Vector3Int(1, 1, 3);
        NewElementData = new Paint[]
        {
            P(1, 2)
        };


        // Act
        Execute(KERNEL_ID_fill);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 3, WriteIndex = 2, Volume = 2.5f }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0.5f, 1),

                P(1, 1),

                P(1, 0.5f),
            },
            PaintGridContentData);
    }

    [Test]
    public void voxel_half_filled_spanning_fill_enough_space_more_cell_volume()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 1, WriteIndex = 0, Volume = 0.5f }
        };
        PaintGridContentData = new Paint[]
        {
            P(0, 0.5f),

            P(-1),
        };
        PaintGridSize = new Vector3Int(1, 1, 2);
        PaintGridCellVolume = 2;
        NewElementData = new Paint[]
        {
            P(1, 2)
        };


        // Act
        Execute(KERNEL_ID_fill);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 2, WriteIndex = 1, Volume = 2.5f }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0.75f, 2),

                P(1, 0.5f),
            },
            PaintGridContentData);
    }

    [Test]
    public void voxel_half_filled_spanning_fill_not_enough_space()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 1, WriteIndex = 0, Volume = 0.5f }
        };
        PaintGridContentData = new Paint[]
        {
            P(0, 0.5f),
        };
        PaintGridSize = new Vector3Int(1, 1, 1);
        NewElementData = new Paint[]
        {
            P(1, 1)
        };


        // Act
        Execute(KERNEL_ID_fill);


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
                P(0.5f, 1),
            },
            PaintGridContentData);
    }

    [Test]
    public void entire_column_only_partially_filled()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 3, WriteIndex = 0, Volume = 2.4f }
        };
        PaintGridContentData = new Paint[]
        {
            P(0, 0.8f),

            P(0, 0.8f),

            P(0, 0.8f),

            P(-1),
        };
        PaintGridSize = new Vector3Int(1, 1, 4);
        NewElementData = new Paint[]
        {
            P(1, 1)
        };


        // Act
        Execute(KERNEL_ID_fill);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 4, WriteIndex = 3, Volume = 3.4f }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0.2f, 1),

                P(0.2f, 1),

                P(0.2f, 1),

                P(1,    0.4f),
            },
            PaintGridContentData);
    }

    [Test]
    public void column_filled_diffuse_depth_one()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 2, WriteIndex = 2, Volume = 2 }
        };
        PaintGridContentData = new Paint[]
        {
            P(0, 1),

            P(0, 1),

            P(-1),
        };
        PaintGridSize = new Vector3Int(1, 1, 3);
        PaintGridDiffuseDepth = 1;
        PaintGridDiffuseRatio = 0.3f;
        NewElementData = new Paint[]
        {
            P(1, 1)
        };


        // Act
        Execute(KERNEL_ID_fill);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 3, WriteIndex = 3, Volume = 3 }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0, 1),

                P(0.3f, 1),

                P(0.7f, 1)
            },
            PaintGridContentData);
    }

    [Test]
    public void column_filled_diffuse_depth_two()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 2, WriteIndex = 2, Volume = 2 }
        };
        PaintGridContentData = new Paint[]
        {
            P(0, 1),

            P(0, 1),

            P(-1),
        };
        PaintGridSize = new Vector3Int(1, 1, 3);
        PaintGridDiffuseDepth = 2;
        PaintGridDiffuseRatio = 0.3f;
        NewElementData = new Paint[]
        {
            P(1, 1)
        };


        // Act
        Execute(KERNEL_ID_fill);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 3, WriteIndex = 3, Volume = 3 }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0.09f, 1),

                P(0.21f, 1),

                P(0.7f, 1)
            },
            PaintGridContentData);
    }

    [Test]
    public void column_filled_diffuse_depth_one_more_on_top()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 2, WriteIndex = 2, Volume = 2 }
        };
        PaintGridContentData = new Paint[]
        {
            P(0, 1),

            P(0, 1),

            P(0),

            P(0),
        };
        PaintGridSize = new Vector3Int(1, 1, 4);
        PaintGridDiffuseDepth = 1;
        PaintGridDiffuseRatio = 0.3f;
        NewElementData = new Paint[]
        {
            P(1, 2)
        };


        // Act
        Execute(KERNEL_ID_fill);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 4, WriteIndex = 4, Volume = 4 }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0, 1),

                P(0.3f, 1),

                P(0.7f, 1),

                P(1, 1),
            },
            PaintGridContentData);
    }

    [Test]
    public void column_filled_diffuse_deeper_than_column_does_nothing()
    {
        // Arrange
        PaintGridInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 1, WriteIndex = 1, Volume = 1 }
        };
        PaintGridContentData = new Paint[]
        {
            P(0, 1),

            P(-1),
        };
        PaintGridSize = new Vector3Int(1, 1, 2);
        PaintGridDiffuseDepth = 2;
        PaintGridDiffuseRatio = 0.3f;
        NewElementData = new Paint[]
        {
            P(1, 1)
        };


        // Act
        Execute(KERNEL_ID_fill);


        // Assert
        Assert.AreEqual(
            new ColumnInfo[]
            {
                new ColumnInfo { Size = 2, WriteIndex = 2, Volume = 2 }
            },
            PaintGridInfoData);

        Assert.AreEqual(
            new Paint[]
            {
                P(0.3f, 1),

                P(0.7f, 1),
            },
            PaintGridContentData);
    }
}