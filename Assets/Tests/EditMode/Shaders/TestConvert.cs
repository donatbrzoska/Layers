using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestConvert
{
    private const int KERNEL_ID_pixel_to_world_space = 0;
    private const int KERNEL_ID_rakel_anchor_to_index_space = 1;

    List<CSAttribute> Attributes;

    [SetUp]
    public void Setup()
    {
        Attributes = new()
        {
            new CSInt("TextureResolution", 2),
            new CSFloat2("SurfaceSize", new Vector2(15, 10))
        };
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        ComputeShaderTask cst = new ComputeShaderTask(
            "Tests/TestConvert",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            Attributes,
            true,
            kernelID);

        cst.Run();

        return cst;
    }

    [Test]
    public void pixel_to_world_space_CanvasCentered_LowerLeft()
    {
        // Arrange
        Attributes.Add(new CSFloat3("SurfacePosition", new Vector3(0, 0, 0)));
        Attributes.Add(new CSInt2("Pixel", new Vector2Int(0, 0)));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_pixel_to_world_space);


        // Assert
        Color c = cst.DebugValues[0];
        Vector3 result = new Vector3(c.r, c.g, c.b);

        Assert.AreEqual(
            new Vector3(-7.25f, -4.75f, 0),
            result);
    }

    [Test]
    public void pixel_to_world_space_CanvasCentered_AlmostCenter()
    {
        // Arrange
        Attributes.Add(new CSFloat3("SurfacePosition", new Vector3(0, 0, 0)));
        Attributes.Add(new CSInt2("Pixel", new Vector2Int(14, 9)));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_pixel_to_world_space);


        // Assert
        Color c = cst.DebugValues[0];
        Vector3 result = new Vector3(c.r, c.g, c.b);

        Assert.AreEqual(
            new Vector3(-0.25f, -0.25f, 0),
            result);
    }

    [Test]
    public void pixel_to_world_space_CanvasShifted_LowerLeft()
    {
        // Arrange
        Attributes.Add(new CSFloat3("SurfacePosition", new Vector3(-2, -1, 0)));
        Attributes.Add(new CSInt2("Pixel", new Vector2Int(0, 0)));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_pixel_to_world_space);


        // Assert
        Color c = cst.DebugValues[0];
        Vector3 result = new Vector3(c.r, c.g, c.b);

        Assert.AreEqual(
            new Vector3(-9.25f, -5.75f, 0),
            result);
    }

    [Test]
    public void rakel_anchor_to_index_space_CanvasCentered_LowerLeft()
    {
        // Arrange
        Attributes.Add(new CSFloat3("WorldSpacePosition", new Vector3(0, 0, 0)));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_rakel_anchor_to_index_space);


        // Assert
        Color c = cst.DebugValues[0];
        Vector2 result = new Vector2(c.r, c.g);

        Assert.AreEqual(
            new Vector2(-0.5f, -0.5f),
            result);
    }

    [Test]
    public void rakel_anchor_to_index_space_CanvasCentered_LowerLeftPixel()
    {
        // Arrange
        Attributes.Add(new CSFloat3("WorldSpacePosition", new Vector3(0.25f, 0.25f, 0)));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_rakel_anchor_to_index_space);


        // Assert
        Color c = cst.DebugValues[0];
        Vector2 result = new Vector2(c.r, c.g);

        Assert.AreEqual(
            new Vector2(0, 0),
            result);
    }

    [Test]
    public void rakel_anchor_to_index_space_CanvasCentered_MiddleRight()
    {
        // Arrange
        Attributes.Add(new CSFloat3("WorldSpacePosition", new Vector3(1, 1, 0)));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_rakel_anchor_to_index_space);


        // Assert
        Color c = cst.DebugValues[0];
        Vector2 result = new Vector2(c.r, c.g);

        Assert.AreEqual(
            new Vector2(1.5f, 1.5f),
            result);
    }
}