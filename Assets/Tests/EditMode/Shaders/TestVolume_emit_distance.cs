using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TestVolume_emit_distance
{
    private const int KERNEL_ID_emit_volume_distance = 0;

    private const float EmitDistance_MAX = 0.1f;

    List<CSAttribute> Attributes;

    [SetUp]
    public void Setup()
    {
        Attributes = new() { };
        Attributes.Add(new CSFloat("EmitDistance_MAX", EmitDistance_MAX));
        Attributes.Add(new CSFloat("EmitVolume_MIN", 0));
        Attributes.Add(new CSFloat("EmitVolume_MAX", 1));
    }

    private ComputeShaderTask Execute(int kernelID)
    {
        ComputeShaderTask cst = new ComputeShaderTask(
            "Tests/TestVolume_emit",
            new ShaderRegion(Vector2Int.zero, Vector2Int.zero, Vector2Int.zero, Vector2Int.zero),
            Attributes,
            true,
            kernelID);

        cst.Run();

        return cst;
    }

    [Test]
    public void emit_volume_distance_Lower_OOB()
    {
        // Arrange
        Attributes.Add(new CSFloat("Distance", -1));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_emit_volume_distance);


        // Assert
        Color c = cst.DebugValues[0];
        float result = c.r;

        Assert.AreEqual(0, result);
    }

    [Test]
    public void emit_volume_distance_Lower()
    {
        // Arrange
        Attributes.Add(new CSFloat("Distance", 0));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_emit_volume_distance);


        // Assert
        Color c = cst.DebugValues[0];
        float result = c.r;

        Assert.AreEqual(0, result);
    }

    [Test]
    public void emit_volume_distance_Middle()
    {
        // Arrange
        Attributes.Add(new CSFloat("Distance", 0.5f * EmitDistance_MAX));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_emit_volume_distance);


        // Assert
        Color c = cst.DebugValues[0];
        float result = c.r;

        Assert.AreEqual(1, result);
    }

    [Test]
    public void emit_volume_distance_Upper()
    {
        // Arrange
        Attributes.Add(new CSFloat("Distance", EmitDistance_MAX));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_emit_volume_distance);


        // Assert
        Color c = cst.DebugValues[0];
        float result = c.r;

        Assert.AreEqual(0, result);
    }

    [Test]
    public void emit_volume_distance_Upper_OOB()
    {
        // Arrange
        Attributes.Add(new CSFloat("Distance", EmitDistance_MAX + 1));


        // Act
        ComputeShaderTask cst = Execute(KERNEL_ID_emit_volume_distance);


        // Assert
        Color c = cst.DebugValues[0];
        float result = c.r;

        Assert.AreEqual(0, result);
    }
}
