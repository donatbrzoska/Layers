using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestPaintGrid_copy
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
            "Tests/TestPaintGrid",
            new ShaderRegion(Vector2Int.zero, new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 1)),
            Attributes,
            true,
            kernelID);

        cst.Run();

        return cst;
    }

    Paint P(float v)
    {
        return new Paint(new Vector3(v, v, v), v);
    }

    [Test]
    public void copy()
    {
        Vector3Int paintGridSize = new Vector3Int(2, 2, 2);

        // Arrange
        ComputeBuffer sourcePGInfo = new ComputeBuffer(paintGridSize.x * paintGridSize.y, ColumnInfo.SizeInBytes);
        ColumnInfo[] sourcePGInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }, new ColumnInfo { Size = 1, WriteIndex = 0, Volume = 0.2f },
            new ColumnInfo { Size = 2, WriteIndex = 1, Volume = 1.4f }, new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 },
        };
        sourcePGInfo.SetData(sourcePGInfoData);
        ComputeBuffer sourcePGContent = new ComputeBuffer(paintGridSize.x * paintGridSize.y * paintGridSize.z, Paint.SizeInBytes);
        Paint[] sourcePGContentData = new Paint[] // negative values should not be copied because out of range (see sourceInfoData)
        {
              P(-10),  P(0.2f),
              P(1f), P(-40),

              P(-20),  P(-30),
              P(0.4f), P(-50),
        };
        sourcePGContent.SetData(sourcePGContentData);

        ComputeBuffer targetPGInfo = new ComputeBuffer(paintGridSize.x * paintGridSize.y, ColumnInfo.SizeInBytes);
        ColumnInfo[] targetPGInfoData = new ColumnInfo[]
        {
            new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }, new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 },
            new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 }, new ColumnInfo { Size = 0, WriteIndex = 0, Volume = 0 },
        };
        targetPGInfo.SetData(targetPGInfoData);
        ComputeBuffer targetPGContent = new ComputeBuffer(paintGridSize.x * paintGridSize.y * paintGridSize.z, Paint.SizeInBytes);
        Paint[] targetPGContentData = new Paint[]
        {
            P(-1), P(-2),
            P(-3), P(-4),

            P(-5), P(-6),
            P(-7), P(-8),
        };
        targetPGContent.SetData(targetPGContentData);

        Attributes.Add(new CSComputeBuffer("SourcePGInfo", sourcePGInfo));
        Attributes.Add(new CSComputeBuffer("SourcePGContent", sourcePGContent));
        Attributes.Add(new CSComputeBuffer("TargetPGInfo", targetPGInfo));
        Attributes.Add(new CSComputeBuffer("TargetPGContent", targetPGContent));
        Attributes.Add(new CSInt3("PGSize", paintGridSize));


        // Act
        Execute(KERNEL_ID_copy);


        // Assert
        targetPGInfo.GetData(targetPGInfoData);
        Assert.AreEqual(
            sourcePGInfoData,
            targetPGInfoData);

        targetPGContent.GetData(targetPGContentData);
        Assert.AreEqual(
            new Paint[]
            {
                P(-1),   P(0.2f),
                P(1),    P(-4),

                P(-5),   P(-6),
                P(0.4f), P(-8),
            },
            targetPGContentData);


        // Cleanup
        sourcePGInfo.Dispose();
        sourcePGContent.Dispose();
        targetPGInfo.Dispose();
        targetPGContent.Dispose();
    }
}