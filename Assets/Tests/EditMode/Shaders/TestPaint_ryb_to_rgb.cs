using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestPaint_ryb_to_rgb
{
    private const int KERNEL_ID_ryb_to_rgb = 3;

    List<CSAttribute> Attributes;

    [SetUp]
    public void Setup()
    {
        Attributes = new List<CSAttribute>();
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        ComputeShaderTask cst = new ComputeShaderTask(
            "Tests/TestPaint",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            Attributes,
            true,
            kernelID);

        cst.Run();

        return cst;
    }

    [Test]
    public void ryb_to_rgb_black()
    {
        // Arrange
        Attributes.Add(new CSFloat3("RYBColor", new Vector3(1, 1, 1)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_ryb_to_rgb);

        // Assert
        Vector3 result = cst.DebugValues[0];

        AssertUtil.AssertColorsAreEqual(
            new Vector3(0, 0, 0),
            result);
    }

    [Test]
    public void ryb_to_rgb_white()
    {
        // Arrange
        Attributes.Add(new CSFloat3("RYBColor", new Vector3(0, 0, 0)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_ryb_to_rgb);

        // Assert
        Vector3 result = cst.DebugValues[0];

        AssertUtil.AssertColorsAreEqual(
            new Vector3(1, 1, 1),
            result);
    }

    [Test]
    public void ryb_to_rgb_grey()
    {
        // Arrange
        Attributes.Add(new CSFloat3("RYBColor", new Vector3(0.5f, 0.5f, 0.5f)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_ryb_to_rgb);

        // Assert
        Vector3 result = cst.DebugValues[0];

        AssertUtil.AssertColorsAreEqual(
            new Vector3(0.5f, 0.5f, 0.5f),
            result);
    }

    [Test]
    public void ryb_to_rgb_red()
    {
        // Arrange
        Attributes.Add(new CSFloat3("RYBColor", new Vector3(1, 0, 0)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_ryb_to_rgb);

        // Assert
        Vector3 result = cst.DebugValues[0];

        AssertUtil.AssertColorsAreEqual(
            new Vector3(1, 0, 0),
            result);
    }

    [Test]
    public void ryb_to_rgb_yellow()
    {
        // Arrange
        Attributes.Add(new CSFloat3("RYBColor", new Vector3(0, 1, 0)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_ryb_to_rgb);

        // Assert
        Vector3 result = cst.DebugValues[0];

        AssertUtil.AssertColorsAreEqual(
            new Vector3(1, 1, 0),
            result);
    }

    [Test]
    public void ryb_to_rgb_blue()
    {
        // Arrange
        Attributes.Add(new CSFloat3("RYBColor", new Vector3(0, 0, 1)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_ryb_to_rgb);

        // Assert
        Vector3 result = cst.DebugValues[0];

        AssertUtil.AssertColorsAreEqual(
            new Vector3(0, 0, 1),
            result);
    }
}
