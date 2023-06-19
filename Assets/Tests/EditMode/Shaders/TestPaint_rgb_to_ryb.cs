using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestPaint_rgb_to_ryb
{
    private const int KERNEL_ID_rgb_to_ryb = 2;

    List<CSAttribute> Attributes;

    [SetUp]
    public void Setup()
    {
        Attributes = new List<CSAttribute>();
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        ComputeShaderTask cst = new ComputeShaderTask(
            "TestPaint",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            Attributes,
            true,
            kernelID);

        cst.Run();

        return cst;
    }

    [Test]
    public void rgb_to_ryb_black()
    {
        // Arrange
        Attributes.Add(new CSFloat4("RGBColor", new Color(0, 0, 0)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_rgb_to_ryb);

        // Assert
        Color result = cst.DebugValues[0];

        AssertUtil.AssertColorsAreEqual(
            new Color(1, 1, 1),
            result);
    }

    [Test]
    public void rgb_to_ryb_white()
    {
        // Arrange
        Attributes.Add(new CSFloat4("RGBColor", new Color(1, 1, 1)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_rgb_to_ryb);

        // Assert
        Color result = cst.DebugValues[0];

        AssertUtil.AssertColorsAreEqual(
            new Color(0, 0, 0),
            result);
    }

    [Test]
    public void rgb_to_ryb_red()
    {
        // Arrange
        Attributes.Add(new CSFloat4("RGBColor", new Color(1, 0, 0)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_rgb_to_ryb);

        // Assert
        Color result = cst.DebugValues[0];

        AssertUtil.AssertColorsAreEqual(
            new Color(1, 0, 0),
            result);
    }

    [Test]
    public void rgb_to_ryb_green()
    {
        // Arrange
        Attributes.Add(new CSFloat4("RGBColor", new Color(0, 1, 0)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_rgb_to_ryb);

        // Assert
        Color result = cst.DebugValues[0];

        AssertUtil.AssertColorsAreEqual(
            new Color(0, 1, 1),
            result);
    }

    [Test]
    public void rgb_to_ryb_blue()
    {
        // Arrange
        Attributes.Add(new CSFloat4("RGBColor", new Color(0, 0, 1)));

        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_rgb_to_ryb);

        // Assert
        Color result = cst.DebugValues[0];

        AssertUtil.AssertColorsAreEqual(
            new Color(0, 0, 1),
            result);
    }
}
